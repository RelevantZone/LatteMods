namespace LatteMods.CustomItems.Items.Firearms
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Attachments;
    using YamlDotNet.Serialization;

    [CustomItem(ItemType.GunFSP9)]
    public class Injector : CustomWeapon
    {
        [YamlIgnore]
        public Dictionary<Player, float> cure = new Dictionary<Player, float>();
        public override uint Id { get; set; } = 2102;
        public override string Name { get; set; } = "Enforcer <b><color=blue>NANO</color></b> Injector";
        public override string Description { get; set; } = "Distributions of <b><color=blue>NANO</color></b> latest technologies, this weapon is filled with nanites that stitch wounds " +
            "for friendlies. Also viable for combat, although the performance is <color=red>drastically worst</color> compared to normal assigned weapons.";
        public override float Weight { get; set; }
        public override float Damage { get; set; } = 2f;
        public override byte ClipSize { get; set; } = 50;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            RoomSpawnPoints =
            {
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.LczGlassBox
                },
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.Lcz914
                },
                new RoomSpawnPoint
                {
                    Chance = 75,
                    Room = Exiled.API.Enums.RoomType.EzGateA
                },
                new RoomSpawnPoint
                {
                    Chance = 75,
                    Room = Exiled.API.Enums.RoomType.EzGateB
                }
            },
        };

        public override AttachmentName[] Attachments { get; set; } = 
        [
            AttachmentName.SoundSuppressor,
            AttachmentName.Laser,
            AttachmentName.DotSight
        ];

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
            ev.Amount = Damage;
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            if (ev.Target == null)
            {
                return;
            }

            if (ev.Player.Role.Side == ev.Target.Role.Side &&
                ev.Target.Role.Side != Exiled.API.Enums.Side.Scp)
            {
                ev.Player.ShowHitMarker();

                if (ev.Target.Health >= ev.Target.MaxHealth)
                {
                    ev.Target.AddAhp(Damage, limit: 250, persistant: true);
                } else
                {
                    ev.Target.Heal(Damage);
                }
            }
        }
    }
}
