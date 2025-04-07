namespace LatteMods.CustomItems.Handlers
{
    using System;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using Events = Exiled.Events.Handlers.Player;
    public class Player
    {
        public void SubscribeEvents()
        {
            Events.ThrownProjectile += OnThrownProjectile;
            Events.Shooting += OnShooting;
        }

        public void UnsubscribeEvents()
        {
            Events.ThrownProjectile -= OnThrownProjectile;
            Events.Shooting -= OnShooting;
        }

        private void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (CustomItem.TryGet(ev.Item, out var customItem))
            {
                ev.Pickup.PickupTime = customItem.Weight;
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (CustomItem.TryGet(ev.Firearm, out CustomItem item) &&
                item is CustomWeapon firearm &&
                ev.Firearm.TotalAmmo > firearm.ClipSize)
            {
                var surplus = (ushort)(ev.Firearm.TotalAmmo - firearm.ClipSize);

                ev.Firearm.MagazineAmmo = Math.Max(firearm.ClipSize - 1, 0);
                if (surplus > 0)
                {
                    ev.Player.AddAmmo(ev.Firearm.AmmoType, surplus);
                }
            }
        }
    }
}
