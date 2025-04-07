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
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp096;
    using Exiled.Events.EventArgs.Server;
    using MEC;
    using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
    using Utils.NonAllocLINQ;
    using YamlDotNet.Serialization;
    using Events = Exiled.Events.Handlers;

    [CustomItem(ItemType.SCP1344)]
    public class Scrambler : CustomItem
    {
        private readonly string _playerSessionKey = $"Player{nameof(Scrambler)}Serial";
        private readonly List<Item> actives = new List<Item>();
        private readonly Dictionary<int, float> batteries = new Dictionary<int, float>();

        [YamlIgnore]
        public readonly List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

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

        public Broadcast MinimumBattery = new Broadcast("Scrambler battery is too <color=red>low</color>!");

        public Broadcast RemainingBattery = new Broadcast("Scrambler battery is currently charged at <b><color={1}>{0}%</color></b>{2}", 5);

        public Broadcast UnequipScrambler = new Broadcast("<color=yellow>Scrambler</color> has been deactivated", 3);

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

        //protected override void SubscribeEvents()
        //{
        //    Events.Server.RoundEnded += OnRoundEnded;
        //    Events.Scp096.AddingTarget += OnAddingTarget;
        //    Events.Player.UsingItem += OnUsingItem;
        //    Events.Player.UsingItemCompleted += OnUsingItemCompleted;
        //    //Events.Player.ChangingItem += OnChangingItem;

        //    base.SubscribeEvents();
        //}

        //protected override void UnsubscribeEvents()
        //{
        //    Events.Server.RoundEnded -= OnRoundEnded;
        //    Events.Scp096.AddingTarget -= OnAddingTarget;
        //    Events.Player.UsingItem -= OnUsingItem;
        //    Events.Player.UsingItemCompleted -= OnUsingItemCompleted;
        //    //Events.Player.ChangingItem -= OnChangingItem;

        //    base.UnsubscribeEvents();
        //}

        //protected override void OnDroppingItem(DroppingItemEventArgs ev)
        //{
        //    if (actives.Contains(ev.Item))
        //    {
        //        actives.Remove(ev.Item);
        //        ev.Player.SessionVariables.Remove(_playerSessionKey);
        //        ev.Player.ReferenceHub.DisableWearables(WearableElements.Scp1344Goggles);
        //    }
        //}

        //protected override void OnChanging(ChangingItemEventArgs ev)
        //{
        //    if (Check(ev.Item))
        //    {
        //        ShowRemainingBattery(ev.Player, ev.Item.Serial, showDescription: true);
        //    };
        //}

        //protected override void OnAcquired(Player player, Item item, bool displayMessage)
        //{
        //    if (! batteries.ContainsKey(item.Serial))
        //    {
        //        GetRemainingBattery(item.Serial);
        //        Timing.RunCoroutine(Update(item), item.Base.gameObject);
        //    }

        //    base.OnAcquired(player, item, displayMessage);
        //}

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var coroutine in coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }

            coroutines.Clear();
        }

        private void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (IsActive(ev.Player) && ev.IsLooking)
            {
                ev.IsAllowed = false;
                return;
            }
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (! Check(ev.Item))
            {
                return;
            }

            if (TryGetActiveItem(ev.Player, out var item))
            {
                ev.IsAllowed = false;
                actives.Remove(item);
                ev.Player.SessionVariables.Remove(_playerSessionKey);
                ev.Player.ReferenceHub.DisableWearables(WearableElements.Scp1344Goggles);
                return;
            }

            if (GetBatteryPercentage(GetRemainingBattery(ev.Item.Serial)) <= MinimumBatteryForUse)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(new Hint(MinimumBattery.Content));
            }
        }

        private void OnUsingItemCompleted(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                return;
            }

            ev.IsAllowed = false;
            actives.Add(ev.Item);
            ev.Player.SessionVariables.Add(_playerSessionKey, ev.Item.Serial);
            ev.Player.ReferenceHub.EnableWearables(WearableElements.Scp1344Goggles);
        }

        private IEnumerator<float> Update(Item item)
        {
            while (true)
            {
                ProcessBattery(item);

                var battery = GetRemainingBattery(item.Serial);

                if (IsActive(item) && battery <= 0)
                {
                    batteries[item.Serial] = 0;
                    actives.Remove(item);
                    item.Owner.SessionVariables.Remove(_playerSessionKey);
                    item.Owner.ReferenceHub.DisableWearables(WearableElements.Scp1344Goggles);
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        private void ProcessBattery(Item item)
        {
            var battery = GetRemainingBattery(item.Serial);
            if (IsActive(item))
            {
                var num = DrainMultiplier;
                if (DrainInPercentage)
                {
                    num = MaxRechargeDuration * DrainMultiplier;
                }

                battery -= num;
                ShowRemainingBattery(item.Owner, battery);
            } else
            {
                battery += 1f;
            }

            batteries[item.Serial] = battery;
        }

        private void ShowRemainingBattery(Player player, float battery, bool showDescription = false)
        {
            var percentage = GetBatteryPercentage(battery);
            var color = "green";
            if (percentage >= MinimumBatteryForUse && percentage < 80)
            {
                color = "yellow";
            }
            else if (percentage < MinimumBatteryForUse)
            {
                color = "red";
            }

            if (RemainingBattery.Show)
            {
                player.ShowHint(new Hint(string.Format(RemainingBattery.Content, percentage, color, showDescription ? $"\n{Description}" : "")));
            }
        }

        private bool IsActive(Item item)
        {
            if (item.Owner != null) return IsActive(item.Owner);
            return false;
        }

        private bool IsActive(Player player)
        {
            if (player.TryGetSessionVariable(_playerSessionKey, out int serial) && actives.Any(x => x.Serial == serial))
            {
                return true;
            }

            return false;
        }

        private bool TryGetActiveItem(Player player, out Item item)
        {
            if (player.TryGetSessionVariable(_playerSessionKey, out int serial) && actives.TryGetFirst(x => x.Serial == serial, out item))
            {
                return true;
            }

            item = null;
            return false;
        }

        private Item GetActiveItem(Player player)
        {
            if (player.TryGetSessionVariable(_playerSessionKey, out int serial) && actives.TryGetFirst(x => x.Serial == serial, out Item item))
            {
                return item;
            }

            return null;
        }

        private float GetRemainingBattery(ushort serial)
        {
            if (!batteries.TryGetValue(serial, out var battery))
            {
                battery = MaxRechargeDuration * DefaultBattery;
                batteries[serial] = battery;
            }
            return battery;
        }

        private double GetBatteryPercentage(float battery)
        {
            return Math.Truncate((battery / MaxRechargeDuration) * 100);
        }
    }
}
