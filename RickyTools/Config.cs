namespace LatteMods.RickyTools
{
    using System.IO;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string Configs = Path.Combine(Paths.Configs, "LatteMods/RickyTools");

        public string GetConfigPath<T>()
        {
            return Path.Combine(Configs, $"{nameof(T).ToLower()}.yml");
        }

        public string GetConfigPath(object config)
        {
            return Path.Combine(Configs, $"{nameof(config).ToLower()}.yml");
        }
        public T ReadConfig<T>() where T : IConfig, new()
        {
            var path = GetConfigPath<T>();

            if (! File.Exists(path))
            {
                return new T();
            }
            return Loader.Deserializer.Deserialize<T>(GetConfigPath<T>());
        }

        public void WriteConfig(object config)
        {
            File.WriteAllText(GetConfigPath(config), Loader.Serializer.Serialize(config));
        }

        // Configs
        public Configs.Items ItemsConfig { get; set; }

        public void LoadConfigs()
        {
            ItemsConfig = ReadConfig<Configs.Items>();
        }
    }
}
