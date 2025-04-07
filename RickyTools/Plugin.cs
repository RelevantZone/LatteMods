namespace LatteMods.RickyTools
{
    using Exiled.API.Enums;
    using Exiled.API.Features;

    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; internal set; }

        // Events

        // Configs
        private Configs.Items ItemsConfig;

        public override string Name { get; } = "LatteMods.RickyTools";
        public override string Prefix { get; } = "LatteMods.RickyTools";
        public override string Author { get; } = "RelevantZone";
        public override PluginPriority Priority => PluginPriority.High;
        public override void OnEnabled()
        {
            Instance = this;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {

            base.OnDisabled();
        }
    }
}
