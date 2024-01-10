using System.Linq;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace MobsRadar;

public class Config
{
    public readonly string Comment = "\"dead\" and \"pet\" are used for not configurable states of entities. You can add your own groups";
    public readonly string CommentIcon = "(icons require full asset path to svg icon, set to null to disable) For example, game:textures/icons/checkmark.svg";
    public readonly string CommentRefreshRate = "(in milliseconds. default: 1000, disable: -1) How often to check for alive status, visibility, horizontal and vertical radius of markers";
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
            RefreshRate = previousConfig.RefreshRate;

            foreach ((string key, EntityMark value) in previousConfig.Markers.Where(keyVal => !Markers.ContainsKey(keyVal.Key)))
            {
                Markers.Add(key, value);
            }

            HorizontalRadius = previousConfig.HorizontalRadius;
            VerticalRadius = previousConfig.VerticalRadius;
        }

        FillDefault();
    }

    private void FillDefault()
    {
        foreach ((string key, EntityMark value) in Core.DefaultMarkers)
        {
            if (!Markers.ContainsKey(key))
            {
                Markers.Add(key, value);
            }
        }
    }
}