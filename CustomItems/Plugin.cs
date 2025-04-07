namespace LatteMods.CustomItems
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using InventorySystem.Items.Firearms.Modules;

    public class Plugin : Plugin<Config>
    {
        private Handlers.Player PlayerEvent;
        public override string Name => "LatteMods.CustomItems";
        public override string Prefix => "LatteMods.CustomItems";
        public override string Author => "RelevantZone";
        public override PluginPriority Priority => PluginPriority.Medium;
        public override Version Version => new Version(1, 0, 1);

        public override void OnEnabled()
        {
            PlayerEvent = new Handlers.Player();

            PlayerEvent.SubscribeEvents();

            RegisterItems();
            base.OnEnabled();
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
        }

        public override void OnDisabled()
        {
            PlayerEvent.UnsubscribeEvents();

            UnregisterItems();
            base.OnDisabled();
        }

        public void RegisterItems()
        {
            Config.LoadConfigs();
            CustomItem.RegisterItems(overrideClass: Config.ItemsConfig);
        }

        // The method provided by lib doesn't work
        public void UnregisterItems()
        {
            for (var index = 0; index < CustomItem.Registered.Count; index++)
            {
                var item = CustomItem.Registered.ElementAt(index);
                if (item is CustomItem customItem)
                {
                    item.Unregister();
                }
            }
        }
    }
}
