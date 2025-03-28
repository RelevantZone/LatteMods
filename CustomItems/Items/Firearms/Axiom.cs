namespace CustomItems.Items.Firearms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using YamlDotNet.Serialization;

    [CustomItem(ItemType.GunCOM18)]
    public class Axiom : CustomWeapon
    {
        private Dictionary<ushort, ProjectileType> LoadedAmmo = [];
        public override uint Id { get; set; } = 2101;
        public override string Name { get; set; } = "<b><color=red>AXIOM</color></b>-5";
        public override string Description { get; set; } = "An improvised distribution of a grenade launcher developed with advanced technologies capable of instantly channeling explosive effects through photon lasers.";
        public override float Damage { get; set; } = 0.5f;
        public override float Weight { get; set; } = 30f;
        public override byte ClipSize { get; set; } = 0;
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
                    Room = Exiled.API.Enums.RoomType.Hcz049
                }
            },
            Limit = 1
        };

        [Description("The ammunition for the weapon will be using grenades, false to only use weaponry ammo")]
        public bool UseGrenades = true;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            Log.Info($"{nameof(Axiom)}: Invoked reloading for weapon");
            
            ev.IsAllowed = true;
            if (UseGrenades)
            {
                Log.Info($"{nameof(Axiom)}: Using grenades as substitute for weapon ammunition");

                ProjectileType? grenadeType = null;

                foreach (var item in ev.Player.Items)
                {
                    if (item.IsThrowable)
                    {
                        switch (item.Type)
                        {
                            case ItemType.GrenadeHE:
                                grenadeType = ProjectileType.FragGrenade;
                                break;
                            case ItemType.GrenadeFlash:
                                grenadeType = ProjectileType.Flashbang;
                                break;
                            case ItemType.SCP018:
                                grenadeType = ProjectileType.Scp018;
                                break;
                            case ItemType.SCP2176:
                                grenadeType = ProjectileType.Scp2176;
                                break;
                        }

                        if (grenadeType != null && grenadeType is ProjectileType type)
                        {
                            Log.Info($"{nameof(Axiom)}: Found ammunition for weapon, destroying item with projectile {Enum.GetName(typeof(ProjectileType), type)}"); ;

                            LoadedAmmo[ev.Firearm.Serial] = type;
                            item.Destroy();
                            break;
                        }
                    }
                }

                if (grenadeType == null)
                {
                    Log.Info($"{nameof(Axiom)}: Initiated reloading sequence, but no ammunition was found");
                    ev.IsAllowed = false;
                }
            }
        }

        protected override void OnReloaded(ReloadedWeaponEventArgs ev)
        {
            // Guard check >:C
            ev.Firearm.MagazineAmmo = 0;
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            ProjectileType grenadeType;
            if (! LoadedAmmo.TryGetValue(ev.Firearm.Serial, out grenadeType))
            {
                grenadeType = ProjectileType.FragGrenade;
            }

            if (Projectile.CreateAndSpawn(grenadeType, ev.Position, previousOwner: ev.Player).Is(out TimeGrenadeProjectile projectile))
            {
                if (grenadeType != ProjectileType.Scp018)
                {
                    projectile.FuseTime = 0.1f;
                }
            }
        }
    }
}
