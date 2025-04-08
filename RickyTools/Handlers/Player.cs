namespace LatteMods.RickyTools.Handlers
{
    using System;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;

    public class Player
    {
        public void OnEscaping(EscapingEventArgs ev)
        {
            if (! (ev.Player != null && ev.Player.IsAlive && ev.Player.Role.Side != Exiled.API.Enums.Side.Scp))
            {
                return;
            }

            switch (ev.Player.Role.Type)
            {
                case PlayerRoles.RoleTypeId.ClassD:
                case PlayerRoles.RoleTypeId.Scientist:
                    break;
                default:
                    if (!ev.Player.IsCuffed)
                    {
                        ev.IsAllowed = false;
                    }
                    break;
            }
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if (CustomItem.TryGet(ev.Firearm, out CustomItem item) && 
                item is CustomWeapon firearm && 
                ev.Firearm.TotalAmmo > firearm.ClipSize)
            {
                ev.Firearm.MagazineAmmo = Math.Max(firearm.ClipSize - 1, 0);
            }
        }
    }
}
