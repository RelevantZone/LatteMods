namespace CustomItems.Items.Usables
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using CustomPlayerEffects;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Usables;
    using MEC;
    using PlayerRoles;
    using UnityEngine;
    using YamlDotNet.Serialization;
    using Events = Exiled.Events.Handlers;
    public enum FortuneType
    {
        [Description("Nothing")]
        None,

        [Description("Supply of basic survival kit, pistol and medkit")]
        Survival,

        [Description("Enabling small gameplay status effects with no duration")]
        Blessed,

        [Description("Ntf loadout, a rifle, combat armor, and medkit")]
        Armed,

        [Description("Changes humans to random scp (except Scp079)")]
        Evolution
    }

    [CustomItem(ItemType.Coin)]
    public class FortuneCoin : CustomItem
    {
        public override uint Id { get; set; } = 4102;
        public override string Name { get; set; } = "<color=yellow>Fortune Coin</color>";
        public override string Description { get; set; } = "A strange man dressed with fine suit once gave this to a foundation personel. " +
            "They say it brings you fortune that guides you to your predetermined future.";
        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            //RoomSpawnPoints =
            //{
            //    new RoomSpawnPoint
            //    {
            //        Chance = 25,
            //        Room = Exiled.API.Enums.RoomType.LczCafe
            //    },
            //    new RoomSpawnPoint
            //    {
            //        Chance = 25,
            //        Room = Exiled.API.Enums.RoomType.LczToilets
            //    },
            //    new RoomSpawnPoint
            //    {
            //        Chance = 25,
            //        Room = Exiled.API.Enums.RoomType.LczGlassBox
            //    }
            //},
            //RoleSpawnPoints =
            //{
            //    new RoleSpawnPoint
            //    {
            //        Chance = 25,
            //        Role = PlayerRoles.RoleTypeId.ClassD
            //    }
            //}
        };

        public float HeadsMultiplier = 1.01f;
        public float TailsMultiplier = 0.92f;

        public Dictionary<FortuneType, float> Rewards = new Dictionary<FortuneType, float>
        {
            [FortuneType.None] = 1,
            [FortuneType.Survival] = 0.75f,
            [FortuneType.Blessed] = 1 / 2500,
            [FortuneType.Armed] = 1 / 1000,
            [FortuneType.Evolution] = 1 / 5000,
        };

        public List<RoleTypeId> Evolutions = [
            RoleTypeId.Scp049,
            RoleTypeId.Scp096,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
            RoleTypeId.Scp939
        ];

        protected override void SubscribeEvents()
        {
            Events.Player.FlippingCoin += OnFlippingCoin;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Events.Player.FlippingCoin -= OnFlippingCoin;

            base.UnsubscribeEvents();
        }

        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            float total = Rewards.Values.Sum(x => x) * (ev.IsTails ? TailsMultiplier : HeadsMultiplier);
            float random = Random.Range(0, total + 1);
            float count = 0;
            
            foreach (var reward in Rewards)
            {
                if (random >= count)
                {
                    count += reward.Value;
                    continue;
                }

                Timing.CallDelayed(1f, () =>
                {
                    if (ev.Player.IsAlive)
                    {
                        ev.Item.Destroy();
                        RewardPlayer(ev.Player, reward.Key);
                    }
                });
                break;
            }
        }

        private void RewardPlayer(Player player, FortuneType type)
        {
            switch (type)
            {
                case FortuneType.None:
                    player.ShowHint("You have been <color=yellow>fortuned</color> to get nothing )\r\n💀)\r\n💀)\r\n💀");
                    break;
                case FortuneType.Survival:
                    player.AddItem(ItemType.GunFSP9);
                    player.AddItem(ItemType.Medkit);
                    player.AddItem(ItemType.ArmorLight);
                    player.AddAmmo(Exiled.API.Enums.AmmoType.Nato9, 120);

                    player.ShowHint("You have been <color=yellow>fortuned</color> to survive this facility madness");
                    break;
                case FortuneType.Armed:
                    player.AddItem(ItemType.GunE11SR);
                    player.AddItem(ItemType.Medkit);
                    player.AddItem(ItemType.ArmorCombat);
                    player.AddAmmo(Exiled.API.Enums.AmmoType.Nato556, 150);

                    player.ShowHint("You have been <color=yellow>fortuned</color> to combat your everlasting opponents");
                    break;
                case FortuneType.Blessed:
                    player.EnableEffect<MovementBoost>(10);
                    player.EnableEffect<DamageReduction>(10);
                    player.EnableEffect<Invigorated>(1);


                    var regenCurve = AnimationCurve.Constant(0, 1f, 1f);
                    var process = new RegenerationProcess(regenCurve, 0.1f, 100f);

                    UsableItemsController.GetHandler(player.ReferenceHub).ActiveRegenerations.Add(process);

                    player.ShowHint("You have been <color=yellow>fortuned</color> with skills to continue your journey");
                    break;
                case FortuneType.Evolution:
                    var random = Random.Range(0, Evolutions.Count);

                    player.Role.Set(Evolutions[random], RoleSpawnFlags.AssignInventory);

                    player.ShowHint("You have been <color=yellow>fortuned</color>\n<b><color=red>fate takes you to a different path</color></b>");
                    break;
            }
        }
    }
}
