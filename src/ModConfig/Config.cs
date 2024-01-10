using System.Linq;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace MobsRadar;

public class Config
{
    public int HorizontalRadius { get; set; } = 999;
    public int VerticalRadius { get; set; } = 20;

    public Dictionary<string, EntityMark> Markers { get; set; } = new();

    public Config()
    {
    }
    public Config(ICoreAPI api, Config previousConfig)
    {
        if (previousConfig != null)
        {
            foreach ((string key, EntityMark value) in previousConfig.Markers.Where(keyVal => !Markers.ContainsKey(keyVal.Key)))
            {
                Markers.Add(key, value);
            }

            HorizontalRadius = previousConfig.HorizontalRadius;
            VerticalRadius = previousConfig.VerticalRadius;
        }

        if (api != null)
        {
            FillDefault(api);
        }
    }

    private void FillDefault(ICoreAPI api)
    {
        Dictionary<string, EntityMark> DefaultMarkers = new()
        {
            ["dead"] = new() { Visible = true, Size = 28, Color = "#ffff00" },
            ["boat"] = new() { Visible = true, Size = 28, Color = "#00aaff" },
            ["trader"] = new() { Visible = true, Size = 24, Color = "#0000ff" },
            ["bug"] = new() { Visible = true, Size = 24, Color = "#777777" },
            ["default"] = new() { Visible = true, Size = 28, Color = "#777777" },
            ["fish"] = new() { Visible = true, Size = 28, Color = "#add8e6" },
            ["hostile"] = new() { Visible = true, Size = 28, Color = "#ff0000" },
            ["item"] = new() { Visible = true, Size = 24, Color = "#ff99ff" },
            ["neutral"] = new() { Visible = true, Size = 28, Color = "#ffa500" },
            ["passive"] = new() { Visible = true, Size = 28, Color = "#00ff00" },
            ["pet"] = new() { Visible = true, Size = 28, Color = "#008000" },
            ["projectile"] = new() { Visible = true, Size = 24, Color = "#00ffff" }
        };

        foreach ((string key, EntityMark value) in DefaultMarkers)
        {
            if (!Markers.ContainsKey(key))
            {
                Markers.Add(key, value);
            }
        }
    }
}