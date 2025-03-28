namespace LatteMod.Customs.Constructions
{
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;

    using Handlers = Exiled.Events.Handlers;
    [CustomItem(ItemType.Medkit)]
    public class Expendable : CustomItem
    {
        public override uint Id { get; set; } = 150003;
        public override string Name { get; set; } = "Expendable \"TDF\" MERC";
        public override string Description { get; set; } = "";
        public override float Weight { get; set; } = 75f;
        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            Handlers.Player.UsingItemCompleted += OnUsingItemCompleted;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Handlers.Player.UsingItemCompleted -= OnUsingItemCompleted;

            base.UnsubscribeEvents();
        }

        public void OnUsingItemCompleted(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                return;
            }

            ev.IsAllowed = false;
            ev.Item.Destroy();
        }
    }
}
