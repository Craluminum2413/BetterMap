using Vintagestory.API.Common;

namespace MobsRadar.Configuration;

public static class ModConfig
{
    private const string ConfigName = "MobsRadarConfig.json";

    public static Config ReadConfig(ICoreAPI api)
    {
        Config config;

        try
        {
            config = LoadConfig(api);

            if (config == null)
            {
                GenerateConfig(api);
                config = LoadConfig(api);
            }
            else
            {
                GenerateConfig(api, config);
            }
        }
        catch
        {
            GenerateConfig(api);
            config = LoadConfig(api);
        }

        return config;
    }

    public static void WriteConfig(ICoreAPI api, Config config) => GenerateConfig(api, config);

    private static Config LoadConfig(ICoreAPI api)
    {
        return api.LoadModConfig<Config>(ConfigName);
    }

    private static void GenerateConfig(ICoreAPI api)
    {
        api.StoreModConfig(new Config(), ConfigName);
    }

    private static void GenerateConfig(ICoreAPI api, Config previousConfig)
    {
        api.StoreModConfig(new Config(api, previousConfig), ConfigName);
    }
}