using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace MobsRadar;

public class RadarSettings
{
    public bool Enabled { get; set; } = true;
    public int HorizontalRadius { get; set; } = 999;
    public int VerticalRadius { get; set; } = 20;
    public double Opacity { get; set; } = 1.0f;
    public List<string> HiddenMarks { get; set; } = new List<string>();

    public Dictionary<string, EntityMark> Markers { get; set; } = new Dictionary<string, EntityMark>()
    {
        ["boat"] = new() { Size = 28, Color = "#00AAFF" },
        ["bugs"] = new() { Size = 24, Color = "#777777" },
        ["default"] = new() { Size = 28, Color = "#777777" },
        ["fish"] = new() { Size = 28, Color = "#add8e6" },
        ["hostile"] = new() { Size = 28, Color = "#FF0000" },
        ["item"] = new() { Size = 24, Color = "#FF99FF" },
        ["pet"] = new() { Size = 28, Color = "#008000" },
        ["neutral"] = new() { Size = 28, Color = "#ffa500" },
        ["passive"] = new() { Size = 28, Color = "#00FF00" },
        ["projectile"] = new() { Size = 24, Color = "#00FFFF" },
    };

    public List<string> GetActiveMarks(ICoreAPI api)
    {
        var availableMarks = api.ModLoader.GetModSystem<Core>().AvailableMarks;
        return availableMarks.Except(HiddenMarks).ToList();
    }
}