using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

[assembly: ModInfo(name: "Mobs Radar", modID: "mobsradar", Side = "Client")]

namespace MobsRadar;

public class MobsRadar : ModSystem
{
    public Dictionary<EnumEntityCategory, ConfigEntityMark> MarkerConfig { get; set; }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        try
        {
            MarkerConfig = api.LoadModConfig<Dictionary<EnumEntityCategory, ConfigEntityMark>>("MobsRadarConfig.json");
            if (MarkerConfig != null && MarkerConfig.Count == Enum.GetValues(typeof(EnumEntityCategory)).Length)
            {
                api.Logger.Notification("MobsRadar Config successfully loaded.");
            }
            else
            {
                MarkerConfig = DefaultConfig;
                api.Logger.Notification("No MobsRadar Config specified. Falling back to default settings");
            }
        }
        catch
        {
            MarkerConfig = DefaultConfig;
            api.Logger.Error("Failed to load custom MobsRadar Config. Falling back to default settings!");
        }
        finally
        {
            api.StoreModConfig(MarkerConfig, "MobsRadarConfig.json");
        }

        api.ModLoader.GetModSystem<WorldMapManager>().RegisterMapLayer<MobsRadarMapLayer>("MobsRadar", 0.5);
        api.Input.RegisterHotKey("mobsradar", Lang.Get("mobsradar:ToggleRadar"), GlKeys.R, HotkeyType.GUIOrOtherControls, ctrlPressed: true);
        api.Input.SetHotKeyHandler("mobsradar", (keycomb) => ToggleRadar(api));

        api.World.Logger.Event("started 'Mobs Radar' mod");
    }

    private bool ToggleRadar(ICoreClientAPI capi)
    {
        var mobsradar = capi.ModLoader.GetModSystem<WorldMapManager>().MapLayers.Find(layer => layer is MobsRadarMapLayer);
        if (mobsradar != null)
        {
            mobsradar.Active = !mobsradar.Active;
            return true;
        }
        return false;
    }

    private static Dictionary<EnumEntityCategory, ConfigEntityMark> DefaultConfig => new Dictionary<EnumEntityCategory, ConfigEntityMark>()
    {
        [EnumEntityCategory.boat] = new() { Size = 28, Color = "#00AAFF", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.bugs] = new() { Size = 24, Color = "#777777", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.other] = new() { Size = 28, Color = "#777777", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.fish] = new() { Size = 28, Color = "#add8e6", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.hostile] = new() { Size = 28, Color = "#FF0000", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.item] = new() { Size = 24, Color = "#FF99FF", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.pet] = new() { Size = 28, Color = "#008000", Visible = true, maxHorizontalDistance = 1000, maxVerticalDistance = 100 },
        [EnumEntityCategory.neutral] = new() { Size = 28, Color = "#ffa500", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.passive] = new() { Size = 28, Color = "#00FF00", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
        [EnumEntityCategory.projectile] = new() { Size = 24, Color = "#00FFFF", Visible = true, maxHorizontalDistance = 100, maxVerticalDistance = 10 },
    };
}
