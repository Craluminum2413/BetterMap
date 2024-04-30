using ConfigLib;
using ImGuiNET;
using MobsRadar.Configuration;
using System;
using System.Linq;
using System.Numerics;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace MobsRadar;

public class ConfigLibCompatibility
{
    private const string settingAdd = "mobsradar:Config.Setting.Add";
    private const string settingClasses = "mobsradar:Config.Setting.Classes";
    private const string settingColor = "mobsradar:Config.Setting.Color";
    private const string settingEnableIcon = "mobsradar:Config.Setting.EnableIcon";
    private const string settingHorizontalRadius = "mobsradar:Config.Setting.HorizontalRadius";
    private const string settingIcon = "mobsradar:Config.Setting.Icon";
    private const string settingMarkerProperties = "mobsradar:Config.Setting.MarkerProperties";
    private const string settingMarkers = "mobsradar:Config.Setting.Markers";
    private const string settingRefreshRate = "mobsradar:Config.Setting.RefreshRate";
    private const string settingRemove = "mobsradar:Config.Setting.Remove";
    private const string settingSize = "mobsradar:Config.Setting.Size";
    private const string settingTypes = "mobsradar:Config.Setting.Types";
    private const string settingVerticalRadius = "mobsradar:Config.Setting.VerticalRadius";
    private const string settingVisible = "mobsradar:Config.Setting.Visible";
    private const string textDefaultTexturePath = "game:textures/";
    private const string textIcon = "mobsradar:Config.Text.Icon";
    private const string textMarkers = "mobsradar:Config.Text.Markers";
    private const string textRefreshRate = "mobsradar:Config.Text.RefreshRate";
    private const string textRegexSupport = "mobsradar:Config.Text.SupportsWildcard";

    public ConfigLibCompatibility(ICoreClientAPI api)
    {
        Init(api);
    }

    private void Init(ICoreClientAPI api)
    {
        api.ModLoader.GetModSystem<ConfigLibModSystem>().RegisterCustomConfig("mobsradar", (id, buttons) => EditConfig(id, buttons, api));
    }

    private void EditConfig(string id, ControlButtons buttons, ICoreClientAPI api)
    {
        if (buttons.Save) ModConfig.WriteConfig(api, Core.Config);
        if (buttons.Defaults) Core.Config = new();
        Edit(api, Core.Config, id);
    }

    private int selectedMarker = 0;
    private string markerToAdd = "";
    private void Edit(ICoreClientAPI api, Configuration.Config config, string id)
    {
        ImGui.TextWrapped(Lang.Get(textMarkers));
        ImGui.TextWrapped(Lang.Get(textRefreshRate));

        int refreshRate = config.RefreshRate;
        ImGui.InputInt(Lang.Get(settingRefreshRate) + $"##refreshRate-{id}", ref refreshRate, step: 1, step_fast: 10);
        config.RefreshRate = refreshRate <= -2 ? -1 : refreshRate;

        int horizontalRadius = config.HorizontalRadius;
        ImGui.InputInt(Lang.Get(settingHorizontalRadius) + $"##horizontalRadius-{id}", ref horizontalRadius, step: 1, step_fast: 10);
        config.HorizontalRadius = horizontalRadius <= 0 ? 1 : horizontalRadius;

        int verticalRadius = config.VerticalRadius;
        ImGui.InputInt(Lang.Get(settingVerticalRadius) + $"##verticalRadius-{id}", ref verticalRadius, step: 1, step_fast: 10);
        config.VerticalRadius = verticalRadius <= 0 ? 1 : verticalRadius;

        bool canRemove = config.Markers.Count > 1;
        if (!canRemove) ImGui.BeginDisabled();
        if (ImGui.Button(Lang.Get(settingRemove) + $"##remove{id}"))
        {
            string key = config.Markers.Keys.ElementAt(selectedMarker);
            config.Markers.Remove(key);
            if (selectedMarker >= config.Markers.Count) selectedMarker = config.Markers.Count - 1;
        }
        if (!canRemove) ImGui.EndDisabled();
        ImGui.SameLine();

        bool canAddMarker = markerToAdd != "" && !config.Markers.ContainsKey(markerToAdd);
        if (!canAddMarker) ImGui.BeginDisabled();
        if (ImGui.Button(Lang.Get(settingAdd) + $"##add-{id}"))
        {
            config.Markers.Add(markerToAdd, new());
            selectedMarker = config.Markers.Keys.ToArray().IndexOf(markerToAdd);
        }
        if (!canAddMarker) ImGui.EndDisabled();
        ImGui.SameLine();

        ImGui.InputTextWithHint($"##{id}", Lang.Get(textRegexSupport), ref markerToAdd, 512);

        string[] keys = config.Markers.Keys.ToArray();
        ImGui.ListBox(Lang.Get(settingMarkers) + $"##markers-{id}", ref selectedMarker, keys, keys.Length);

        ImGui.SeparatorText(Lang.Get(settingMarkerProperties));
        try
        {
            config.Markers[keys[selectedMarker]] = EditMarker(config.Markers[keys[selectedMarker]], id);
        }
        catch (Exception) { }
    }

    public EntityMark EditMarker(EntityMark entityMark, string id)
    {
        bool visible = entityMark.Visible;
        ImGui.Checkbox(Lang.Get(settingVisible) + $"##visible-{id}", ref visible);
        entityMark.Visible = visible;

        bool iconEnabled = entityMark.Icon != null;
        ImGui.Checkbox(Lang.Get(settingEnableIcon) + $"##iconEnabled-{id}", ref iconEnabled);
        if (iconEnabled)
        {
            ImGui.TextWrapped(Lang.Get(textIcon));
            ImGui.Indent();
            AssetLocation icon = new(entityMark.Icon ?? textDefaultTexturePath);
            VSImGui.AssetLocationEditor.Edit(Lang.Get(settingIcon) + $"##icon-{id}", ref icon, icon);
            entityMark.Icon = icon.ToString();
            ImGui.Unindent();
        }
        else
        {
            entityMark.Icon = null;
        }

        int size = entityMark.Size;
        ImGui.SliderInt(Lang.Get(settingSize) + $"##size-{id}", ref size, 1, 128);
        entityMark.Size = size;

        double[] color4 = ColorUtil.Hex2Doubles(entityMark.Color ?? "#FFFFFF");
        Vector3 color3 = new((float)color4[0], (float)color4[1], (float)color4[2]);
        ImGui.ColorEdit3(Lang.Get(settingColor) + $"##color-{id}", ref color3, ImGuiColorEditFlags.DisplayHex);
        entityMark.Color = ColorUtil.Doubles2Hex(new double[] { color3[0], color3[1], color3[2] });

        try
        {
            string types = entityMark.MatchTypes.Length > 0 ? entityMark.MatchTypes.Aggregate((first, second) => $"{first}\n{second}") : "";
            ImGui.InputTextMultiline(Lang.Get(settingTypes) + $"##types-{id}", ref types, 2048, new(0, 0));
            entityMark.MatchTypes = types.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            string classes = entityMark.MatchClasses.Length > 0 ? entityMark.MatchClasses.Aggregate((first, second) => $"{first}\n{second}") : "";
            ImGui.InputTextMultiline(Lang.Get(settingClasses) + $"##classes-{id}", ref classes, 2048, new(0, 0));
            entityMark.MatchClasses = classes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }
        catch (Exception) { }

        return entityMark;
    }
}