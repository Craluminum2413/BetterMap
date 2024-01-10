using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

[assembly: ModInfo(name: "Mobs Radar", modID: "mobsradar")]

namespace MobsRadar;

public class Core : ModSystem
{
    public static Config Config { get; set; }
    public static Dictionary<string, EntityMark> DefaultMarkers { get; set; }

    public override void AssetsLoaded(ICoreAPI api)
    {
        DefaultMarkers = api.Assets.TryGet("mobsradar:config/markers_default.json").ToObject<Dictionary<string, EntityMark>>();
        Config = ModConfig.ReadConfig(api);
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        WorldMapManager worldMapManager = api.ModLoader.GetModSystem<WorldMapManager>();
        worldMapManager.RegisterMapLayer<MobsRadarMapLayer>("MobsRadar", 0.5);

        api.Input.RegisterHotKey("mobsradarconfig", Lang.Get("mobsradar:UpdateRadarConfig"), GlKeys.R, HotkeyType.GUIOrOtherControls, shiftPressed: true);
        api.Input.SetHotKeyHandler("mobsradarconfig", (keycomb) => UpdateRadarConfig(api));

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    private static bool UpdateRadarConfig(ICoreClientAPI capi)
    {
        Config = ModConfig.ReadConfig(capi);

        MobsRadarMapLayer mobsradar = capi.ModLoader.GetModSystem<WorldMapManager>().MapLayers.Find(layer => layer is MobsRadarMapLayer) as MobsRadarMapLayer;
        if (Config.RefreshRate == -1)
        {
            mobsradar.Dispose();
            mobsradar.UpdateTextures();
            mobsradar.UpdateMarkers();
        }
        return true;
    }
}
