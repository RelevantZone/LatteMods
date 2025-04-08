namespace LatteMods.RickyTools.Configs
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    [Description("Applies temporary patches for exiled")]
    public class Patches : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
    }
}
