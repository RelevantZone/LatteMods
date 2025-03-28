namespace CustomItems.Items.Usables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
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

    internal class ScramblerState
    {
        public int Serial;
        public Player Owner;
        public float Battery;
        public bool IsActive;

        internal ScramblerState(int serial, float battery = 50)
        {
            Serial = serial;
            Battery = battery;
            IsActive = false;
        }
    }

    [CustomItem(ItemType.SCP1344)]
    public class Scrambler : CustomItem
    {
        [YamlIgnore]
        private readonly List<ScramblerState> states = new List<ScramblerState>();

        [YamlIgnore]
        public readonly List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public override uint Id { get; set; } = 4101;
        public override string Name { get; set; } = "ENOCH <color=red>\"FOX\"</color> SCRAMBLER";
        public override string Description { get; set; } = "Distributed to Nine-Tailed Fox units for previous missions of containing the anomaly designated as Scp096. It's specifically designed to hinder hardly a visual to the creatures face. \n<color=red>Precautions! The anomaly can be agitated by other means than looking onto his face.</color>";
        public override float Weight { get; set; } = 20f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            RoomSpawnPoints =
            {
                new RoomSpawnPoint
                {
                    Chance = 100,
                    Room = Exiled.API.Enums.RoomType.Hcz096
                }
            }
        };

        public Broadcast MinimumBattery = new Broadcast("Scrambler battery is too <color=red>low</color>!");

        public Broadcast RemainingBattery = new Broadcast("Scrambler battery is currently charged at <b><color={1}>{0}%</color></b>{2}", 5);

        public Broadcast UnequipScrambler = new Broadcast("<color=yellow>Scrambler</color> has been deactivated", 3);

        [Description("How long does it take to recharge from zero to max battery.")]
        public float MaxRechargeDuration = 120f;

        [Description("Battery usage when it is used by a player per seconds")]
        public float DrainMultiplier = 0.01f;

        [Description("If battery usage was to be calculated by percentage (ActiveUsagePerSeconds / MaxRechargeDuration)")]
        public bool DrainInPercentage = true;

        [Description("The minimum battery in percentage for using scrambler")]
        public float MinimumBatteryForUse = 20f;

        protected override void SubscribeEvents()
        {
            Events.Server.RoundStarted += OnRoundStarted;
            Events.Server.RoundEnded += OnRoundEnded;
            Events.Scp096.AddingTarget += OnAddingTarget;
            Events.Player.UsingItem += OnUsingItem;
            Events.Player.UsingItemCompleted += OnUsingItemCompleted;
            Events.Player.ChangingItem += OnChangingItem;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Events.Server.RoundStarted -= OnRoundStarted;
            Events.Server.RoundEnded -= OnRoundEnded;
            Events.Scp096.AddingTarget -= OnAddingTarget;
            Events.Player.UsingItem -= OnUsingItem;
            Events.Player.UsingItemCompleted -= OnUsingItemCompleted;
            Events.Player.ChangingItem -= OnChangingItem;

            base.UnsubscribeEvents();
        }

        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            if (Check(ev.Item))
            {
                ShowRemainingBattery(ev.Player, ev.Item.Serial, showDescription: true);
            };
        }

        private void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (! Check(ev.Item))
            {
                if (IsActive(ev.Player) && states.TryGetFirst(x => x.IsActive == true && x.Owner == ev.Player, out var state))
                {
                    DisableScrambler(state);
                }
                return;
            }
        }

        private void OnRoundStarted()
        {
            coroutines.Add(Timing.RunCoroutine(Update()));
        }

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
            }
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (! Check(ev.Item))
            {
                return;
            }

            if (IsActive(ev.Player, forceDisable: true))
            {
                ev.IsAllowed = false;
                ev.Player.CurrentItem = null;
                return;
            }

            var state = GetState(ev.Item.Serial);
            if (ToPercentage(state.Battery, MaxRechargeDuration) <= MinimumBatteryForUse)
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

            var state = GetState(ev.Item.Serial);
            state.IsActive = true;
            state.Owner = ev.Player;

            ev.Player.ReferenceHub.EnableWearables(WearableElements.Scp1344Goggles);
        }

        private IEnumerator<float> Update()
        {
            while (! Round.IsEnded)
            {
                foreach (var state in states)
                {
                    ProcessBattery(state);

                    if (state.Battery <= 0)
                    {
                        state.Battery = 0;
                        DisableScrambler(state);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        private void ProcessBattery(ScramblerState state)
        {
            if (state.IsActive)
            {
                state.Battery -= DrainInPercentage ? (MaxRechargeDuration * DrainMultiplier) : DrainMultiplier;

                if (state.Owner != null) ShowRemainingBattery(state.Owner, state);
            }
            else
            {
                state.Battery = Math.Min(state.Battery + 1, MaxRechargeDuration);
            }
        }

        private ScramblerState GetState(int serial)
        {
            if (! states.TryGetFirst(x => x.Serial == serial, out var state))
            {
                state = new ScramblerState(serial, battery: MaxRechargeDuration / 2);
                states.Add(state);
            }

            return state;
        }

        private bool IsActive(Player player, bool forceDisable = false)
        {
            if (! (player.IsAlive && player.IsHuman && player.Items.Count > 0))
            {
                return false;
            }

            foreach (var item in player.Items)
            {
                if (! Check(item))
                {
                    continue;
                }

                var state = GetState(item.Serial);
                if (state.IsActive)
                {
                    if (forceDisable)
                    {
                        DisableScrambler(state);
                    }
                    return true;
                }
            }

            return false;
        }

        private void DisableScrambler(ScramblerState state)
        {
            if (state.Owner != null)
            {
                state.Owner.ShowHint(UnequipScrambler.Content);
                state.Owner.ReferenceHub.DisableWearables(WearableElements.Scp1344Goggles);
            }

            state.Owner = null;
            state.IsActive = false;
        }

        private void ShowRemainingBattery(Player player, int itemSerial, bool showDescription = false) => ShowRemainingBattery(player, GetState(itemSerial), showDescription);

        private void ShowRemainingBattery(Player player, ScramblerState state, bool showDescription = false)
        {
            var battery = ToPercentage(state.Battery, MaxRechargeDuration);
            var color = "green";
            if (battery >= MinimumBatteryForUse && battery < 80)
            {
                color = "yellow";
            }
            else if (battery < MinimumBatteryForUse)
            {
                color = "red";
            }

            if (RemainingBattery.Show)
            {
                if (showDescription)
                {
                    player.ShowHint(new Hint(string.Format(RemainingBattery.Content, Math.Truncate((battery / MaxRechargeDuration) * 100), color, $"\n{Description}")));
                }
                else
                {
                    player.ShowHint(new Hint(string.Format(RemainingBattery.Content, Math.Truncate((battery / MaxRechargeDuration) * 100), color, "")));
                }
            }
        }

        public static double ToPercentage(float remaining, float max)
        {
            return Math.Truncate((remaining / max) * 100);
        }
    }
}
