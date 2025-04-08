namespace LatteMods.RickyTools
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Events.Commands.Reload;
    using Utils.NonAllocLINQ;

    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; internal set; }
        public override string Name { get; } = "LatteMods.RickyTools";
        public override string Prefix { get; } = "LatteMods.RickyTools";
        public override string Author { get; } = "RelevantZone";
        public override PluginPriority Priority => PluginPriority.High;

        // Events

        // Configs
        public List<IConfig> Configs { get; internal set; } = [];

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
