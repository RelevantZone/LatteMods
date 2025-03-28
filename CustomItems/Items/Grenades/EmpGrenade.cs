namespace CustomItems.Items.Grenades
{
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Map;
    using YamlDotNet.Serialization;

    [CustomItem(ItemType.GrenadeFlash)]
    public class EmpGrenade : CustomGrenade
    {
        public override uint Id { get; set; } = 3101;
        public override string Name { get; set; } = "Tactical EMP Grenade";
        public override string Description { get; set; } = "Produces a pulse of energy that creates a powerful electromagnetic field capable of short-circuiting a wide range of electronic equipment.";
        public override float Weight { get; set; } = 25f;
        public override float FuseTime { get; set; } = 1.5f;
        public override bool ExplodeOnCollision { get; set; } = true;
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
                    Room = Exiled.API.Enums.RoomType.LczArmory
                },
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.Lcz173
                },
                new RoomSpawnPoint
                {
                    Chance = 50,
                    Room = Exiled.API.Enums.RoomType.HczHid
                }
            },
            Limit = 2
        };

        [Description("Duration from the effect of emp grenade will last.")]
        public float EffectDuration { get; set; } = 10f;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            Room room = Room.FindParentRoom(ev.Projectile.GameObject);

            foreach (Door door in room.Doors)
            {
                if (door.IsLocked || door.IsPartOfCheckpoint || door.IsGate)
                {
                    if (door.IsGate && door.Type == Exiled.API.Enums.DoorType.Scp079First)
                    {
                        continue;
                    }

                    door.IsOpen = true;
                    continue;
                }

                door.IsOpen = false;
                door.Lock(EffectDuration, Exiled.API.Enums.DoorLockType.NoPower);
            }

            foreach (Player player in Player.List)
            {
                if (player.Role.Is(out Scp079Role role) && role.Camera != null && role.Camera.Room == room)
                {
                    role.LoseSignal(EffectDuration);
                }
            }
        }
    }
}
