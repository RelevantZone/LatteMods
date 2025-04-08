namespace LatteMods.CustomItems.Items.Usables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using MEC;

    [CustomItem(ItemType.SCP1344)]
    public class Scrambler : CustomItem
    {
        private readonly Dictionary<int, float> batteries = [];
        private readonly List<CoroutineHandle> coroutines = [];

        public override uint Id { get; set; } = 4101;
        public override string Name { get; set; } = "ENOCH <color=red>\"FOX\"</color> SCRAMBLER";
        public override string Description { get; set; } = "Distributed to Nine-Tailed Fox units for previous missions of containing the anomaly designated as Scp096. It's specifically designed to hinder hardly a visual to the creatures face. \n<color=red>Precautions! The anomaly can be agitated by other means than looking onto his face.</color>";
        public override float Weight { get; set; } = 20f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            //RoomSpawnPoints =
            //{
            //    new RoomSpawnPoint
            //    {
            //        Chance = 100,
            //        Room = Exiled.API.Enums.RoomType.Hcz096
            //    }
            //}
        };

        public Broadcast MinimumBattery = new("Scrambler battery is too <color=red>low</color>!");

        public Broadcast RemainingBattery = new("Scrambler battery is currently charged at <b><color={1}>{0}%</color></b>", 5);

        public Broadcast UnequipScrambler = new("<color=yellow>Scrambler</color> has been deactivated", 3);

        [Description("How long does it take to recharge from zero to max battery.")]
        public float MaxRechargeDuration = 120f;

        [Description("Battery usage when it is used by a player per seconds")]
        public float DrainMultiplier = 0.04f;

        [Description("If battery usage was to be calculated by percentage (ActiveUsagePerSeconds / MaxRechargeDuration)")]
        public bool DrainInPercentage = true;

        [Description("The minimum battery in percentage for using scrambler")]
        public double MinimumBatteryForUse = 20.0;

        [Description("The percentage of battery that will be charged for first-time use")]
        public float DefaultBattery = 0.5f;




        protected override void OnWaitingForPlayers()
        {
            batteries.Clear();
            coroutines.Clear();
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (! batteries.ContainsKey(item.Serial))
            {
                batteries.Add(item.Serial, MaxRechargeDuration);
            }

            base.OnAcquired(player, item, displayMessage);
        }

        protected override void ShowSelectedMessage(Player player)
        {
            ShowRemainingBattery(player, batteries.FirstOrDefault(x => x.Key == player.CurrentItem.Serial).Value);
        }




        private void ShowRemainingBattery(Player player, float battery)
        {
            player.ShowHint(string.Format(RemainingBattery.Content, GetBatteryPercentage(battery)));
        }

        private double GetBatteryPercentage(int serial)
        {
            if (! batteries.TryGetValue(serial, out var battery))
            {
                return 0;
            }

            return GetBatteryPercentage(battery);
        }

        private double GetBatteryPercentage(float value)
        {
            return Math.Truncate((value / MaxRechargeDuration) * 100);
        }

        private IEnumerator<float> Update()
        {

        }
    }
}
