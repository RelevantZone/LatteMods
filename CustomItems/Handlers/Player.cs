namespace CustomItems.Handlers
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
            if (CustomItem.TryGet(ev.Item, out var customItem) && customItem is CustomWeapon firearm)
            {
                if (ev.Firearm.TotalAmmo > firearm.ClipSize)
                {
                    int num1 = Math.Max(ev.Firearm.TotalAmmo - (firearm.ClipSize - 1), 0);
                    if (num1 > 0)
                    {
                        ev.Player.AddAmmo(ev.Firearm.AmmoType, (ushort)(ev.Firearm.MagazineAmmo - num1));
                    }
                    ev.Firearm.MagazineAmmo = num1;
                }
            }
        }
    }
}
