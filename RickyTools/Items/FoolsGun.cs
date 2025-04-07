namespace LatteMods.RickyTools.Items
{
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    [CustomItem(ItemType.GunRevolver)]
    public class FoolsGun : CustomWeapon
    {
        public override uint Id { get; set; } = 100000001;
        public override string Name { get; set; } = "<b><color=aqua>Fool's Gun</color></b>";
        public override string Description { get; set; } = "";
        public override float Damage { get; set; } = 150;
        public override float Weight { get; set; } = 450f;
        public override byte ClipSize { get; set; }
        public override SpawnProperties SpawnProperties { get; set; }
        public PlayerPermissions[] RequiredPermissions { get; set; } =
        [
            PlayerPermissions.PlayersManagement,
            PlayerPermissions.FacilityManagement,
            PlayerPermissions.AdminChat,
            PlayerPermissions.Noclip,
            PlayerPermissions.LongTermBanning
        ];

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (! (player.CheckPermission(RequiredPermissions) || ))
            {

            }
            base.OnAcquired(player, item, displayMessage);
        }
    }
}
