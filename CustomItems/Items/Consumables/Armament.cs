namespace LatteMods.CustomItems.Items.Consumables
{
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using PlayerRoles;
    using UnityEngine;
    using YamlDotNet.Serialization;
    using Events = Exiled.Events.Handlers;

    [CustomItem(ItemType.Medkit)]
    public class Armament : CustomItem
    {
        public override uint Id { get; set; } = 1101;
        public override string Name { get; set; } = "Personal Armament Kit";
        public override string Description { get; set; } = "\"Time to take inventory\" - A wise oldman said." +
            "\nPersonal kits equipped with military weapons and armors suited for modern warfare. " +
            "This will be handy when it comes to your survival.";
        public override float Weight { get; set; } = 30f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            RoomSpawnPoints =
            {
                new RoomSpawnPoint
                {
                    Chance = 25,
                    Room = Exiled.API.Enums.RoomType.LczArmory
                },
                new RoomSpawnPoint
                {
                    Chance = 15,
                    Room = Exiled.API.Enums.RoomType.HczArmory
                }
            },
            RoleSpawnPoints =
            {
                new RoleSpawnPoint
                {
                    Chance = 1,
                    Role = PlayerRoles.RoleTypeId.ClassD
                },
                new RoleSpawnPoint
                {
                    Chance = 1,
                    Role = PlayerRoles.RoleTypeId.Scientist
                },
                new RoleSpawnPoint
                {
                    Chance = 5,
                    Role = PlayerRoles.RoleTypeId.FacilityGuard
                }
            },
        };
        public override Vector3 Scale { get; set; } = new Vector3(3, 3, 3);

        public List<ItemType> ArmamentFinalKit { get; set; } = [
            ItemType.ArmorHeavy,
            ItemType.GunFRMG0,
            ItemType.Adrenaline,
            ItemType.SCP500,
            ItemType.Jailbird,
            ItemType.MicroHID,
            ItemType.ParticleDisruptor,
            ItemType.Radio
        ];

        protected override void SubscribeEvents()
        {
            Events.Player.UsingItemCompleted += OnUsingItemCompleted;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Events.Player.UsingItemCompleted -= OnUsingItemCompleted;

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
            ev.Player.DropItems();

            switch (ev.Player.Role.Type)
            {
                case RoleTypeId.ClassD:
                    ev.Player.Role.Set(RoleTypeId.ChaosConscript, RoleSpawnFlags.AssignInventory);
                    break;
                case RoleTypeId.Scientist:
                    ev.Player.Role.Set(RoleTypeId.NtfSpecialist, RoleSpawnFlags.AssignInventory);
                    break;
                case RoleTypeId.FacilityGuard:
                    ev.Player.Role.Set(RoleTypeId.NtfSergeant, RoleSpawnFlags.AssignInventory);
                    break;
                case RoleTypeId.NtfPrivate:
                case RoleTypeId.NtfSergeant:
                case RoleTypeId.NtfSpecialist:
                    ev.Player.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.AssignInventory);
                    break;
                case RoleTypeId.ChaosRifleman:
                case RoleTypeId.ChaosMarauder:
                case RoleTypeId.ChaosConscript:
                    ev.Player.Role.Set(RoleTypeId.ChaosRepressor, RoleSpawnFlags.AssignInventory);
                    break;
                case RoleTypeId.NtfCaptain:
                case RoleTypeId.ChaosRepressor:
                case RoleTypeId.Tutorial:
                    ev.Player.AddAhp(250, limit: 250, persistant: true);
                    ev.Player.Heal(ev.Player.MaxHealth);
                    ev.Player.EnableEffect<DamageReduction>(25);

                    foreach (var item in ArmamentFinalKit)
                    {
                        ev.Player.AddItem(item);
                    }
                    break;
            }
        } 
    }
}
