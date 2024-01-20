using System.Linq;
using System.Collections.Generic;
using Vintagestory.API.Common;
using ImGuiNET;
using Vintagestory.API.Util;

namespace MobsRadar;

public class Config
{
    public const string Comment = "\"dead\" and \"pet\" are used for not configurable states of entities. You can add your own groups";
    public const string CommentIcon = "(icons require full asset path to svg icon, set to null to disable) For example, game:textures/icons/checkmark.svg";
    public const string CommentRefreshRate = "(in milliseconds. default: 1000, disable: -1) How often to check for alive status, visibility, horizontal and vertical radius of markers";
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

    private int selectedMarker = 0;
    private string markerToAdd = "";
    public void Edit(string id)
    {
        ImGui.TextWrapped(Comment);
        ImGui.TextWrapped(CommentRefreshRate);
        bool refreshRateEnabled = RefreshRate != -1;
        ImGui.Checkbox($"Enable refresh rate##{id}", ref refreshRateEnabled);
        if (refreshRateEnabled)
        {
            int refreshRate = RefreshRate;
            ImGui.SliderInt($"Refresh rate (ms)##{id}", ref refreshRate, 1, 10000, "", ImGuiSliderFlags.Logarithmic);
            RefreshRate = refreshRate;
        }
        else
        {
            RefreshRate = -1;
        }

        int horizontalRadius = HorizontalRadius;
        ImGui.SliderInt($"Horizontal radius##{id}", ref horizontalRadius, 1, 10000, "", ImGuiSliderFlags.Logarithmic);
        HorizontalRadius = horizontalRadius;

        int verticalRadius = VerticalRadius;
        ImGui.SliderInt($"Vertical radius##{id}", ref verticalRadius, 1, 1000, "", ImGuiSliderFlags.Logarithmic);
        VerticalRadius = verticalRadius;

        bool canRemove = Markers.Count > 1;
        if (!canRemove) ImGui.BeginDisabled();
        if (ImGui.Button($"Remove##{id}"))
        {
            string key = Markers.Keys.ElementAt(selectedMarker);
            Markers.Remove(key);
            if (selectedMarker >= Markers.Count) selectedMarker = Markers.Count - 1;
        }
        if (!canRemove) ImGui.EndDisabled();
        ImGui.SameLine();

        bool canAddMarker = markerToAdd != "" && !Markers.ContainsKey(markerToAdd);
        if (!canAddMarker) ImGui.BeginDisabled();
        if (ImGui.Button($"Add##{id}"))
        {
            Markers.Add(markerToAdd, new());
            selectedMarker = Markers.Keys.ToArray().IndexOf(markerToAdd);
        }
        if (!canAddMarker) ImGui.EndDisabled();
        ImGui.SameLine();

        ImGui.InputTextWithHint($"##{id}", "supports wildcards and regexes", ref markerToAdd, 512);

        string[] keys = Markers.Keys.ToArray();
        ImGui.ListBox($"Markers##{id}", ref selectedMarker, keys, keys.Length);

        ImGui.SeparatorText("Marker properties");
        Markers[keys[selectedMarker]].Edit(id);
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