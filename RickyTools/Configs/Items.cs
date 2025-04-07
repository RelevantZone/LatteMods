namespace LatteMods.RickyTools.Configs
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    public class Items : IConfig
    {

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Allows bypass for usage of custom items restrictions only for this plugin")]
        public string[] BypassUserIds { get; set; } =
        [
            "steam@76561198270970428",
            "steam@76561199150690309"
        ];
    }
}
