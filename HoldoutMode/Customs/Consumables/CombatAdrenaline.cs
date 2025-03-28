namespace LatteMod.Customs.Consumables
{
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using PlayerEvent = Exiled.Events.Handlers.Player;

    [CustomItem(ItemType.Adrenaline)]
    public class CombatAdrenaline : CustomItem
    {
        public override uint Id { get; set; } = 151001;
        public override string Name { get; set; } = "INVIL Combat Adrenaline";
        public override string Description { get; set; } =
            "Designed by combat medical company INVIL for use by combat infantry combining adrenaline and chemical compounds such as X-21 and YZ-11. The shot is designed to enhance physical attributes of a soldier, namely their mobility and ability to handle weaponry, reducing felt recoil and improving reload speed.";
        public override float Weight { get; set; } = 3f;
        public override SpawnProperties SpawnProperties { get; set; }
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            PlayerEvent.UsingItemCompleted += OnUsingItemCompleted;
        }
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            PlayerEvent.UsingItemCompleted -= OnUsingItemCompleted;
        }

        public void OnUsingItemCompleted(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            ev.IsAllowed = false;
            ev.Item.Destroy();

            ev.Player.EnableEffect<Scp1853>(duration: 30f, intensity: 4);
            ev.Player.EnableEffect<MovementBoost>(duration: 30f, intensity: 25);
            ev.Player.EnableEffect<Invigorated>(duration: 30f);
        }
    }
}
