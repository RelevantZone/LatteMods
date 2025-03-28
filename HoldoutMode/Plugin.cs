namespace LatteMod
{
    using System;
    using System.IO;
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Loader;
    using InventorySystem.Items.Usables;

    public class Plugin : Plugin<Config>
    {
        public override string Name => "LatteMod";
        public override string Author => "none";
        public override string Prefix => "latte";
        public override PluginPriority Priority => PluginPriority.Medium;
        public override Version Version => new(1, 0);

        public override void OnEnabled()
        {
            base.OnEnabled();

            RegisterItems();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            UnregisterItems();
        }

        public void RegisterItems()
        {
            Config.LoadConfigs();

            CustomItem.RegisterItems(overrideClass: Config.ItemsConfig);
        }

        public void UnregisterItems()
        {
            CustomItem.UnregisterItems();
        }
    }
}
