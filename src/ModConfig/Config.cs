using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

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
        if (previousConfig != null)
        {
            foreach ((string key, EntityMark value) in previousConfig.Markers.Where(keyVal => !Markers.ContainsKey(keyVal.Key)))
            {
                Markers.Add(key, value);
            }
        }

        RefreshRate = previousConfig.RefreshRate;
        HorizontalRadius = previousConfig.HorizontalRadius;
        VerticalRadius = previousConfig.VerticalRadius;

        FillDefault();
    }

    private void FillDefault()
    {
        foreach ((string key, EntityMark value) in Core.DefaultMarkers.Where(keyVal => !Markers.ContainsKey(keyVal.Key)))
        {
            Markers.Add(key, value);
        }
    }
}