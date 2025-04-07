namespace LatteMods.CustomItems.Items.Usables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.EventArgs;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using MEC;

    using KnobSetting = Scp914.Scp914KnobSetting;
    using Events = Exiled.Events.Handlers.Player;

    [CustomItem(ItemType.Radio)]
    public class RadioIntercom : CustomItem
    {
        public override uint Id { get; set; } = 4103;
        public override string Name { get; set; } = "<color=green>Radio Intercom</color>";
        public override string Description { get; set; } = "Inter-communication device for sending voice messages throughout the whole foundation facility and infrastructure.";
        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; }

        [Description("The amount of battery this custom item will use")]
        public float DrainMultiplier { get; set; } = 1f;

        protected override void SubscribeEvents()
        {
            Events.UsingRadioBattery += OnUsingRadioBattery;
            Events.TogglingRadio += OnTogglingRadio;
            Events.ChangingRadioPreset += OnChangingRadioPreset;
            Events.DroppedItem += OnDroppedItem;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Events.UsingRadioBattery -= OnUsingRadioBattery;
            Events.TogglingRadio -= OnTogglingRadio;
            Events.ChangingRadioPreset -= OnChangingRadioPreset;
            Events.DroppedItem -= OnDroppedItem;

            base.UnsubscribeEvents();
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (item is Radio radio)
            {
                radio.IsEnabled = false;
                radio.Range = RadioRange.Ultra;
            }

            base.OnAcquired(player, item, displayMessage);
        }

        protected override void OnUpgrading(UpgradingItemEventArgs ev)
        {
            if (ev.KnobSetting == KnobSetting.Rough || ev.KnobSetting == KnobSetting.Coarse)
            {
                return;
            }

            if (ev.Item is Radio radio)
            {
                radio.BatteryLevel = 100;
            }
        }

        private void OnTogglingRadio(TogglingRadioEventArgs ev)
        {
            if (ev.Radio.IsEnabled)
            {
                PlayerRoles.Voice.Intercom.TrySetOverride(ev.Player.ReferenceHub, true);
            } else
            {
                PlayerRoles.Voice.Intercom.TrySetOverride(ev.Player.ReferenceHub, false);
            }
        }

        private void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
        {
            ev.Drain = DrainMultiplier;
        }

        private void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev)
        {
            if (Check(ev.Item))
            {
                ev.NewValue = RadioRange.Ultra;
            }
        }

        private void OnDroppedItem(DroppedItemEventArgs ev)
        {
            if (Check(ev.Pickup) && ev.Pickup is RadioPickup pickup)
            {
                pickup.IsEnabled = false;
            }
        }
    }
}
