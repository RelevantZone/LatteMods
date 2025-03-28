namespace CustomItems
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using InventorySystem.Items.Firearms.Modules;

    public class Plugin : Plugin<Config>
    {
        private Handlers.Player PlayerEvent;
        public override string Name => "LatteMods.CustomItems";
        public override string Prefix => "LatteMods.CustomItems";
        public override string Author => "RelevantZone";
        public override PluginPriority Priority => PluginPriority.Medium;

        public override void OnEnabled()
        {
            PlayerEvent = new Handlers.Player();

            Config.LoadConfigs();
            CustomItem.RegisterItems(overrideClass: Config.ItemsConfig);

            PlayerEvent.SubscribeEvents();
            base.OnEnabled();
        }

        public override void OnReloaded()
        {
            Config.LoadConfigs();

            base.OnReloaded();
        }

        public override void OnDisabled()
        {
            PlayerEvent.UnsubscribeEvents();
            base.OnDisabled();
        }
    }
}
