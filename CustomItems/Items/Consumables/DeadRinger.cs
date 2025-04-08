namespace LatteMods.CustomItems.Items.Consumables
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.EventArgs;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using UnityEngine;

    using Events = Exiled.Events.Handlers;
    public class DeadRinger : CustomItem
    {
        private readonly List<Player> _holders = [];
        private readonly List<Player> _effected = [];
        public override uint Id { get; set; } = 1103;
        public override string Name { get; set; } = "Dead Ringer";
        public override string Description { get; set; } = "";
        public override float Weight { get; set; }
        public override Vector3 Scale { get; set; } = Vector3.one * 3;
        public override SpawnProperties SpawnProperties { get; set; }

        //E
        public float Duration { get; set; } = 5f;

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            _holders.Add(player);
            base.OnAcquired(player, item, displayMessage);
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            _holders.Remove(ev.Player);
        }

        protected override void OnOwnerDying(OwnerDyingEventArgs ev)
        {
            _effected.Remove(ev.Player);
            _holders.Remove(ev.Player);
        }

        protected override void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
            if (ev.ShouldPreserveInventory) return;
            _holders.Remove(ev.Player);
        }

        protected override void OnWaitingForPlayers()
        {
            _holders.Clear();
            _effected.Clear();
        }

        protected override void SubscribeEvents()
        {
            Events.Player.Hurting += OnHurting;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Events.Player.Hurting -= OnHurting;
            base.UnsubscribeEvents();
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (!_holders.Contains(ev.Player)) return;

            if (TryGetRinger(ev.Player, out var ringer) && 
                Ragdoll.TryCreate(ev.Player.Role.Type, ev.Player.DisplayNickname, ev.DamageHandler, out var ragdoll))
            {
                ev.Amount = 0f;
                _effected.Add(ev.Player);
                ringer.Destroy();

                // Intended to not keep invisibility if lost
                ev.Player.EnableEffect<CustomPlayerEffects.Invisible>(duration: Duration);
                ev.Player.EnableEffect<CustomPlayerEffects.MovementBoost>(15, duration: Duration);
                ev.Player.EnableEffect<CustomPlayerEffects.DamageReduction>(100, duration: Duration);

                Timing.CallDelayed(Duration, () =>
                {
                    ragdoll.Destroy();
                });
            }

            _holders.Remove(ev.Player);
        }

        internal bool TryGetRinger(Player player, out Item ringer)
        {
            for(var index = 0; index < player.Items.Count; index++)
            {
                var item = player.Items.ElementAt(index);
                if (Check(item))
                {
                    ringer = item;
                    return true;
                }
            }

            ringer = null;
            return false;
        }
    }
}
