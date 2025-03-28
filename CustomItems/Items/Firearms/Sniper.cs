namespace CustomItems.Items.Firearms
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Attachments;
    using PlayerRoles;

    [CustomItem(ItemType.GunE11SR)]
    public class Sniper : CustomWeapon
    {
        public override uint Id { get; set; } = 2103;
        public override string Name { get; set; } = "Enforcer SR-<b><color=yellow>Echo</color></b>";
        public override string Description { get; set; } = "Improvised rifle equipped with <b>JHP Mk.3</b> anti-personel rounds, these type of rounds are very strong against flesh opponents.";
        public override float Weight { get; set; }
        public override float Damage { get; set; }
        public override byte ClipSize { get; set; } = 2;
        public override AttachmentName[] Attachments { get; set; } = new AttachmentName[5] {
            AttachmentName.ScopeSight,
            AttachmentName.Laser,
            AttachmentName.ExtendedBarrel,
            AttachmentName.SoundSuppressor,
            AttachmentName.AmmoCounter,
        };
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            RoomSpawnPoints =
            {
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.HczArmory
                },
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.HczHid
                }
            },
            Limit = 1
        };

        
        [Description("The default amount of damage that will be multiplied against opponents")]
        public float DamageModifier = 12.5f;

        [Description("The amount of damage that will be multiplied for SCPs, defaults to 5f")]
        public Dictionary<RoleTypeId, float> ScpDamageModifiers = new Dictionary<RoleTypeId, float>
        {
            [RoleTypeId.Scp0492] = 8.5f,
            [RoleTypeId.Scp173] = 0.5f,
            [RoleTypeId.Scp106] = 0.5f
        };

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            float mod = DamageModifier;

            if (ev.Player.IsScp && !ScpDamageModifiers.TryGetValue(ev.Player.Role.Type, out mod))
            {
                mod = 5f;
            }

            ev.Amount *= mod;
            base.OnHurting(ev);
        }

        protected override void OnReloaded(ReloadedWeaponEventArgs ev)
        {
            ev.Firearm.MagazineAmmo = 1;
        }
    }
}
