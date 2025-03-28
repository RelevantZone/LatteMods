namespace LatteMod.Customs.Constructions
{
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using UnityEngine;
    using PlayerEvent = Exiled.Events.Handlers.Player;

    [CustomItem(ItemType.Medkit)]
    public class InfraredMine : CustomItem
    {
        public override uint Id { get; set; } = 150002;
        public override string Name { get; set; } = "O.D.D S-Mine";
        public override string Description { get; set; } = "Attachable explosive device that triggers upon contact detected with infrared laser.";
        public override float Weight { get; set; } = 5f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            PlayerEvent.UsingItemCompleted += CreateInfraredMine;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            PlayerEvent.UsingItemCompleted -= CreateInfraredMine;
        }

        public void CreateInfraredMine(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item)) return;

            ev.IsAllowed = false;
            
            if (Physics.Raycast(ev.Player.Position, ev.Player.CameraTransform.forward, out RaycastHit hitInfo, 5))
            {
                
            }
        }
    }
}
