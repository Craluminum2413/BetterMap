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

    public List<string> GetActiveMarks(ICoreAPI api)
    {
        var availableMarks = api.ModLoader.GetModSystem<Core>().AvailableMarks;
        return availableMarks.Except(HiddenMarks).ToList();
    }
}