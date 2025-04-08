namespace LatteMods.CustomItems.Items.Grenades
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;
    using Utils.NonAllocLINQ;
    using Events = Exiled.Events.Handlers;

    
    [CustomItem(ItemType.GrenadeHE)]
    public class DetonatedCharges : CustomGrenade
    {
        public static DetonatedCharges Instance { get; private set; }

        private readonly List<Pickup> _active = [];
        public override uint Id { get; set; } = 3102;
        public override string Name { get; set; } = "R.C. Detonated Charge";
        public override string Description { get; set; }
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 9999f;
        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; }

        [Description("Requires a detonator to be held on hand to activate charges")]
        public bool RequireDetonator { get; set; } = true;

        [Description("The tool that will be used as a detonator")]
        public ItemType DetonatorTool { get; set; } = ItemType.Radio;

        protected override void SubscribeEvents()
        {
            Instance = this;

            Events.Player.Shooting += OnShooting;
            Events.Player.Died += RemoveActiveCharges;
            Events.Player.Destroying += RemoveActiveCharges;
            Events.Player.ChangingItem += OnChangingItem;
            Events.Player.TogglingNoClip += OnTogglingNoClip;
            
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Events.Player.Shooting -= OnShooting;
            Events.Player.Died -= RemoveActiveCharges;
            Events.Player.Destroying -= RemoveActiveCharges;
            Events.Player.ChangingItem -= OnChangingItem;
            Events.Player.TogglingNoClip -= OnTogglingNoClip;

            base.UnsubscribeEvents();
        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            ev.Pickup.PreviousOwner = ev.Player;

            _active.Add(ev.Pickup);
            base.OnThrownProjectile(ev);
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            _active.Remove(ev.Projectile);

            base.OnExploding(ev);
        }

        protected override void OnWaitingForPlayers()
        {
            _active.Clear();
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            Vector3 forward = ev.Player.CameraTransform.forward;
            if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500))
            {

                EffectGrenade grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();
                if (grenade == null)
                {
                    return;
                }

                var pickup = Pickup.Get(grenade);
                if (_active.Contains(pickup))
                {
                    pickup.Destroy();
                }
            }
        }

        private void RemoveActiveCharges(IPlayerEvent ev)
        {
            var list = _active.Where(x => x.PreviousOwner == ev.Player).ToList();
            for (var index = 0; index < list.Count; index++)
            {
                list[index].Destroy();
            }
        }

        private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (! ev.Player.IsAlive || ! RequireDetonator || ev.Player.IsNoclipPermitted || !HasActiveCharges(ev.Player))
            {
                return;
            }

            if (ev.Player.CurrentItem != null && ev.Player.CurrentItem.Type == DetonatorTool)
            {
                DetonateAllCharges(ev.Player);
            }
        }

        private void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (! (ev.Player.IsAlive && HasActiveCharges(ev.Player)))
            {
                return;
            }

            if (RequireDetonator && ev.Item.Type == DetonatorTool)
            {
                if (TryGet(ev.Item, out _))
                {
                    return;
                }

                ev.Player.ShowHint(new Hint("Press ALT to detonate charges remotely"));
            }
        }

        public void DetonateCharge(Pickup charge)
        {
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0.1f;
            grenade.SpawnActive(charge.Position, owner: charge.PreviousOwner);

            _active.Remove(charge);
            charge.Destroy();
        }

        public void DetonateAllCharges(Player player)
        {
            if (! HasActiveCharges(player))
            {
                return;
            }

            var list = _active.Where(x => x.PreviousOwner == player).ToList();
            for (var index = 0; index < list.Count; index++)
            {
                var charge = list[index];
                DetonateCharge(charge);
            }
        }

        public bool HasActiveCharges(Player player)
        {
            return _active.Any(x => x.PreviousOwner == player);
        }
    }
}
