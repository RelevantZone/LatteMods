namespace LatteMods.CustomItems.Items.Firearms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using YamlDotNet.Serialization;
    using InventorySystem.Items.Usables;
    using UnityEngine;

    [CustomItem(ItemType.GunCOM18)]
    public class Axiom : CustomWeapon
    {
        private readonly ItemType[] _festiveItems = new ItemType[3]
        {
            ItemType.Coal,
            ItemType.SpecialCoal,
            ItemType.Snowball
        };
        private Dictionary<ushort, ProjectileType> _loaded = [];
        public override uint Id { get; set; } = 2101;
        public override string Name { get; set; } = "<b><color=red>AXIOM</color></b>-5";
        public override string Description { get; set; } = "An improvised distribution of a grenade launcher developed with advanced technologies capable of instantly channeling explosive effects through photon lasers.";
        public override float Damage { get; set; } = 0.5f;
        public override float Weight { get; set; } = 30f;
        public override byte ClipSize { get; set; } = 1;
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

        [Description("Ignore custom grenades used as ammo")]
        public bool IgnoreModdedGrenades = true;

        [Description("Include throwable festive items such as coal, rock, snowball, etc.")]
        public bool IgnoreFestiveItems = true;

        [Description("Apply cooldown for weapon to prevent spamming")]
        public float Cooldown = 0f;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            _loaded.Clear();

            base.OnWaitingForPlayers();
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (item is Firearm firearm && firearm.TotalAmmo == ClipSize && ! _loaded.TryGetValue(firearm.Serial, out var type))
            {
                _loaded.Add(item.Serial, ProjectileType.FragGrenade);
            }

            base.OnAcquired(player, item, displayMessage);
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            Log.Info($"{nameof(Axiom)}: Tracked reloading weapon");
            if (_loaded.ContainsKey(ev.Firearm.Serial))
            {
                ev.IsAllowed = false;
                return;
            }

            Log.Info($"{nameof(Axiom)}: Invoked reloading for weapon");
            
            if (UseGrenades)
            {
                Log.Info($"{nameof(Axiom)}: Using grenades as substitute for weapon ammunition");

                ProjectileType type = ProjectileType.None;

                foreach (var item in ev.Player.Items)
                {
                    if (TryGetProjectile(item, out type))
                    {
                        _loaded[ev.Firearm.Serial] = type;
                        item.Destroy();
                        break;
                    }
                }

                if (type == ProjectileType.None)
                {
                    Log.Info($"{nameof(Axiom)}: Initiated reloading sequence, but no ammunition was found");
                    ev.IsAllowed = false;
                }
            }
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            if (!_loaded.TryGetValue(ev.Firearm.Serial, out var type))
            {
                if (!UseGrenades)
                {
                    type = ProjectileType.FragGrenade;
                } else
                {
                    type = ProjectileType.None;
                }
            }

            if (type != ProjectileType.None &&
                Projectile.CreateAndSpawn(type, ev.Position, previousOwner: ev.Player).Is(out TimeGrenadeProjectile projectile))
            {
                if (type != ProjectileType.Scp018)
                {
                    projectile.FuseTime = 0.1f;
                }

                if (Cooldown > 0)
                {
                    UsableItemsController.GlobalItemCooldowns[ev.Firearm.Serial] = Time.timeSinceLevelLoad + Cooldown;
                }

                _loaded.Remove(ev.Item.Serial);
                ev.Player.ShowHitMarker(size: 3);
            }
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        internal bool TryGetProjectile(Item item, out ProjectileType type)
        {
            type = ProjectileType.None;

            if (item.IsThrowable)
            {
                if (IgnoreModdedGrenades && TryGet(item, out var _) ||
                    (IgnoreFestiveItems && _festiveItems.Contains(item.Type))) return false;

                type = item.Type.GetProjectileType();
                return true;
            }

            return false;
        }
    }
}
