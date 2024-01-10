using System.Linq;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace MobsRadar;

public class Config
{
    public readonly string RefreshRateComment = "(in milliseconds. default: 1000, disable: -1) How often to check for alive status, visibility, horizontal and vertical radius of markers";
    public int RefreshRate { get; set; } = 1000;

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
            RefreshRate = previousConfig.RefreshRate;

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
            ["boat"] = new() { Icon = null, Visible = true, Size = 28, Color = "#00aaff" },
            ["bug"] = new() { Icon = null, Visible = true, Size = 24, Color = "#777777" },
            ["dead"] = new() { Icon = null, Visible = true, Size = 28, Color = "#ffff00" },
            ["default"] = new() { Icon = null, Visible = true, Size = 28, Color = "#777777" },
            ["fish"] = new() { Icon = null, Visible = true, Size = 24, Color = "#add8e6" },
            ["hostile"] = new() { Icon = null, Visible = true, Size = 28, Color = "#ff0000" },
            ["item"] = new() { Icon = null, Visible = true, Size = 24, Color = "#ff99ff" },
            ["neutral"] = new() { Icon = null, Visible = true, Size = 28, Color = "#ffa500" },
            ["passive"] = new() { Icon = null, Visible = true, Size = 28, Color = "#00ff00" },
            ["pet"] = new() { Icon = null, Visible = true, Size = 28, Color = "#008000" },
            ["projectile"] = new() { Icon = null, Visible = true, Size = 24, Color = "#00ffff" },
            ["trader"] = new() { Icon = null, Visible = true, Size = 28, Color = "#0000ff" }
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