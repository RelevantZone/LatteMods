namespace CustomItems.Handlers
{
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using Events = Exiled.Events.Handlers.Player;
    public class Player
    {
        public void SubscribeEvents()
        {
            Events.ThrownProjectile += OnThrownProjectile;
        }

        public void UnsubscribeEvents()
        {
            Events.ThrownProjectile -= OnThrownProjectile;
        }

        private void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (CustomItem.TryGet(ev.Item, out var customItem))
            {
                ev.Pickup.PickupTime = customItem.Weight;
            }
        }
    }
}
