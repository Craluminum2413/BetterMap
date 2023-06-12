using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

[assembly: ModInfo("Mobs Radar")]

namespace MobsRadar;

public class Core : ModSystem
{
    private ICoreClientAPI _capi;

    public List<string> AvailableMarks { get; set; }
    public SettingsFile<RadarSettings> RadarSetttings { get; set; } = new(Path.Combine(GamePaths.ModConfig, "MobsRadarConfig.json"));

    public bool IsEnabled() => RadarSetttings.Settings.Enabled;
    public int GetHorizontalRadius() => RadarSetttings.Settings.HorizontalRadius;
    public int GetVerticalRadius() => RadarSetttings.Settings.VerticalRadius;

    public override void AssetsLoaded(ICoreAPI api)
    {
        AvailableMarks = api.Assets.Get(new AssetLocation("mobsradar:config/markers.json")).ToObject<List<string>>();
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        _capi = api;

        var worldMapManager = api.ModLoader.GetModSystem<WorldMapManager>();
        worldMapManager.RegisterMapLayer<MobsRadarMapLayer>("Enemies");

        api.Input.RegisterHotKey("mobsradar", Lang.Get("mobsradar:Action.ToggleRadar"), GlKeys.R, HotkeyType.GUIOrOtherControls, ctrlPressed: true);
        api.Input.SetHotKeyHandler("mobsradar", ToggleRadar);

        var parsers = api.ChatCommands.Parsers;
        api.ChatCommands.Create("mradar")
        .WithAlias("radar")
        .BeginSubCommand("info")
            .HandleWith(PrintRadarInfo)
        .EndSubCommand()
        .BeginSubCommand("hr")
            .WithArgs(parsers.Int("horizontal radius"))
            .HandleWith(x => SetRadius(x, EnumRadius.Horizontal))
        .EndSubCommand()
        .BeginSubCommand("vr")
            .WithArgs(parsers.Int("vertical radius"))
            .HandleWith(x => SetRadius(x, EnumRadius.Vertical))
        .EndSubCommand()
        .BeginSubCommand("opacity")
            .WithArgs(parsers.Float("markers opacity"))
            .HandleWith(SetOpacity)
        .EndSubCommand()
        .BeginSubCommand("hide")
            .WithArgs(parsers.Word("marker name to hide/show"))
            .HandleWith(HideOrShowMarker)
        .EndSubCommand();

        api.World.Logger.Event("started 'Mobs Radar' mod");
    }

    private bool ToggleRadar(KeyCombination t1)
    {
        RadarSetttings.Settings.Enabled = !RadarSetttings.Settings.Enabled;
        RadarSetttings.Save();
        return true;
    }

    private TextCommandResult PrintRadarInfo(TextCommandCallingArgs args)
    {
        var sb = new StringBuilder();

        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.HorizontalRadius", RadarSetttings.Settings.HorizontalRadius.ToString()));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.VerticalRadius", RadarSetttings.Settings.VerticalRadius.ToString()));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.Opacity", RadarSetttings.Settings.Opacity.ToString()));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.AvailableMarks", string.Join(" ", AvailableMarks)));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.VisibleMarks", string.Join(" ", RadarSetttings.Settings.GetActiveMarks(_capi))));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.HiddenMarks", string.Join(" ", RadarSetttings.Settings.HiddenMarks)));

        return TextCommandResult.Success(sb.ToString());
    }

    private TextCommandResult SetRadius(TextCommandCallingArgs args, EnumRadius radius)
    {
        var num = (int)args[0];

        switch (radius)
        {
            case EnumRadius.Horizontal:
                RadarSetttings.Settings.HorizontalRadius = num;
                RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Horizontal radius", num));
            case EnumRadius.Vertical:
                RadarSetttings.Settings.VerticalRadius = num;
                RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Vertical radius", num));
            default:
                return TextCommandResult.Deferred;
        }
    }

    private TextCommandResult SetOpacity(TextCommandCallingArgs args)
    {
        var num = (float)args.LastArg;
        RadarSetttings.Settings.Opacity = num;
        RadarSetttings.Save();

        var worldMapManager = _capi.ModLoader.GetModSystem<WorldMapManager>();
        var mobsRadarMapLayer = (MobsRadarMapLayer)worldMapManager.MapLayers.Single(p => p is MobsRadarMapLayer);
        mobsRadarMapLayer.InitializeTextures();

        return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Opacity", num));
    }

    private TextCommandResult HideOrShowMarker(TextCommandCallingArgs args)
    {
        string word = args[0].ToString();
        List<string> hiddenMarks = RadarSetttings.Settings.HiddenMarks;

        if (!string.IsNullOrEmpty(word) && AvailableMarks.Contains(word))
        {
            if (hiddenMarks.Contains(word))
            {
                hiddenMarks.Remove(word);
                RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.MarkerNowVisible", word));
            }
            else
            {
                hiddenMarks.Add(word);
                RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.MarkerNowHidden", word));
            }
        }
        else
        {
            return TextCommandResult.Error(Lang.Get("mobsradar:Error.WordNotAvailable", word));
        }
    }

}
