using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatteMod
{
    using System.IO;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
        public string Configs = Path.Combine(Paths.Configs, "LatteMods");
        public string ItemConfigFile = "items.yml";

        public Configs.Items ItemsConfig { get; private set; }
        public void LoadConfigs()
        {
            string itemConfig = Path.Combine(Configs, ItemConfigFile);
            if (! Directory.Exists(Configs))
            {
                Directory.CreateDirectory(Configs);
            }

            if (File.Exists(itemConfig))
            {
                ItemsConfig = Loader.Deserializer.Deserialize<Configs.Items>(File.ReadAllText(itemConfig));
                File.WriteAllText(itemConfig, Loader.Serializer.Serialize(ItemsConfig));
            } else
            {
                ItemsConfig = new Configs.Items();
                File.WriteAllText(itemConfig, Loader.Serializer.Serialize(ItemsConfig));
            }
        }
    }
}
