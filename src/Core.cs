using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

[assembly: ModInfo("Mobs Radar")]

namespace MobsRadar;

public class Core : ModSystem
{
    public List<string> AvailableMarks { get; set; }
    public SettingsFile<RadarSettings> RadarSetttings { get; set; } = new(Path.Combine(GamePaths.ModConfig, "MobsRadarConfig.json"));

    public bool IsEnabled() => RadarSetttings.Settings.Enabled;
    public int GetHorizontalRadius() => RadarSetttings.Settings.HorizontalRadius;
    public int GetVerticalRadius() => RadarSetttings.Settings.VerticalRadius;

    public override void AssetsLoaded(ICoreAPI api)
    {
        AvailableMarks = api.Assets.Get(new AssetLocation("mobsradar:config/markers.json")).ToObject<List<string>>();
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        var worldMapManager = api.ModLoader.GetModSystem<WorldMapManager>();
        worldMapManager.RegisterMapLayer<MobsRadarMapLayer>("MobsRadar");

        api.World.Logger.Event("started 'Mobs Radar' mod");
    }
}
