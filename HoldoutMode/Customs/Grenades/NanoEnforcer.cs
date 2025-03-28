namespace LatteMod.Customs.Grenades
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Extensions;
    using PlayerEvent = Exiled.Events.Handlers.Player;

    [CustomItem(ItemType.GrenadeFlash)]
    public class NanoEnforcer : CustomGrenade
    {
        public List<Player> effectedPlayers = new List<Player>();
        public override uint Id { get; set; } = 154001;
        public override string Name { get; set; } = "Enforcer NANO Grenade";
        public override string Description { get; set; } = "Chemical grenade developed for use by Enforcers, disperses nanite filled chemicals in an area that can be programmed for a multitude of uses mainly focusing on area denial.";
        public override float Weight { get; set; } = 5f;
        public override bool ExplodeOnCollision {  get; set; } = true;
        public override float FuseTime { get; set; } = 1.5f;
        public override SpawnProperties SpawnProperties { get; set; }

        public float ExplosionRadius = 7f;
        // Misc
        protected override void SubscribeEvents()
        {
            PlayerEvent.ChangingItem += OnChangingItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerEvent.ChangingItem -= OnChangingItem;
            base.UnsubscribeEvents();
        }

        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (effectedPlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            base.OnExploding(ev);

            effectedPlayers = ListPool<Player>.Pool.Get();

            foreach (var player in ev.TargetsToAffect)
            {
                //if (player.LeadingTeam != ev.Player.LeadingTeam || Server.FriendlyFire)
                //{
                    effectedPlayers.Add(player);
                    ev.Player.EnableEffect<CustomPlayerEffects.Ensnared>(duration: 5f);

                    ev.Player.CurrentItem = null;
                //}
            }
            ListPool<Player>.Pool.Return(effectedPlayers);
        }
    }
}
