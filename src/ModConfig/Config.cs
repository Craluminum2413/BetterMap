using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace MobsRadar.Configuration;

public class Config
{
    public int RefreshRate { get; set; } = 1000;
    public int HorizontalRadius { get; set; } = 999;
    public int VerticalRadius { get; set; } = 20;

    public Dictionary<string, EntityMark> Markers { get; set; } = new();

    public Config()
    {
        FillDefault();
    }

    public Config(ICoreAPI api, Config previousConfig)
    {
        if (previousConfig?.Markers != null)
        {
            Markers.AddRange(previousConfig.Markers);
        }

        RefreshRate = previousConfig?.RefreshRate ?? default;
        HorizontalRadius = previousConfig?.HorizontalRadius ?? default;
        VerticalRadius = previousConfig?.VerticalRadius ?? default;

        FillDefault();
    }

    private void FillDefault()
    {
        Markers.AddRange(Core.DefaultMarkers);
    }
}