using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace MobsRadar;

public class RadarCommands : ModSystem
{
    private ICoreClientAPI _capi;
    private Core GetCore() => _capi.ModLoader.GetModSystem<Core>();

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        _capi = api;

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
        .BeginSubCommand("mark")
            .WithArgs(parsers.Word("marker name to hide/show"))
            .HandleWith(HideOrShowMarker)
        .EndSubCommand()
        .BeginSubCommand("size")
            .WithArgs(parsers.Word("marker name"), parsers.Int("size"))
            .HandleWith(SetMarkerSize)
        .EndSubCommand()
        .BeginSubCommand("color")
            .WithArgs(parsers.Word("marker name"), parsers.Word("color"))
            .HandleWith(SetMarkerColor)
        .EndSubCommand();
    }

    private TextCommandResult PrintRadarInfo(TextCommandCallingArgs args)
    {
        var sb = new StringBuilder();

        sb.AppendLine(Lang.Get(
            "mobsradar:Radar.Radius0",
            Lang.Get("mobsradar:Horizontal"),
            Lang.Get("mobsradar:Vertical"),
            GetCore().RadarSetttings.Settings.HorizontalRadius.ColorLightBlue(),
            GetCore().RadarSetttings.Settings.VerticalRadius.ColorLightBlue()));

        sb.AppendLine(Lang.Get("mobsradar:Radar.Opacity0", GetCore().RadarSetttings.Settings.Opacity.ColorLightBlue()));

        PrintMarkerSizeColor(sb);

        return TextCommandResult.Success(sb.ToString());
    }

    private TextCommandResult SetRadius(TextCommandCallingArgs args, EnumRadius radius)
    {
        var num = (int)args[0];

        var horizontalText = Lang.Get("mobsradar:Horizontal");
        var verticalText = Lang.Get("mobsradar:Vertical");

        switch (radius)
        {
            case EnumRadius.Horizontal:
                GetCore().RadarSetttings.Settings.HorizontalRadius = num;
                GetCore().RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.RadiusSet", horizontalText, num.ColorLightBlue()));
            case EnumRadius.Vertical:
                GetCore().RadarSetttings.Settings.VerticalRadius = num;
                GetCore().RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.RadiusSet", verticalText, num.ColorLightBlue()));
            default:
                return TextCommandResult.Deferred;
        }
    }

    private TextCommandResult SetOpacity(TextCommandCallingArgs args)
    {
        var num = (float)args.LastArg;
        GetCore().RadarSetttings.Settings.Opacity = num;
        GetCore().RadarSetttings.Save();
        InitializeTextures();
        return TextCommandResult.Success(Lang.Get("mobsradar:Success.OpacitySet", num.ColorLightBlue()));
    }

    private TextCommandResult SetMarkerSize(TextCommandCallingArgs args)
    {
        var name = args[0].ToString();
        var size = (int)args[1];

        if (!GetCore().AvailableMarks.Contains(name))
        {
            return TextCommandResult.Error(Lang.Get("mobsradar:Error.WordNotAvailable", name.ColorLightBlue()));
        }

        GetCore().RadarSetttings.Settings.Markers[name].Size = size;
        GetCore().RadarSetttings.Save();
        InitializeTextures();
        return TextCommandResult.Success(Lang.Get("mobsradar:Success.MarkerSizeSet", name.ColorLightBlue(), size.ColorLightBlue()));
    }

    private TextCommandResult SetMarkerColor(TextCommandCallingArgs args)
    {
        var name = args[0].ToString();
        var color = args[1].ToString();

        if (color.Contains('#')) color = color.Replace("#", "");

        if (!GetCore().AvailableMarks.Contains(name))
        {
            return TextCommandResult.Error(Lang.Get("mobsradar:Error.WordNotAvailable", name));
        }

        bool isValidHex = int.TryParse(color, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);

        if (!isValidHex)
        {
            return TextCommandResult.Error(Lang.Get("mobsradar:Error.NotValidHex", color));
        }

        color = color.Insert(0, "#");

        GetCore().RadarSetttings.Settings.Markers[name].Color = color;
        GetCore().RadarSetttings.Save();
        InitializeTextures();

        var sb = new StringBuilder();
        sb.AppendFormat(Lang.Get("mobsradar:Success.MarkerColorSet", name.ColorLightBlue(), color.GetSelfColoredText())).AppendLine();

        return TextCommandResult.Success();
    }

    private TextCommandResult HideOrShowMarker(TextCommandCallingArgs args)
    {
        string word = args[0].ToString();
        List<string> hiddenMarks = GetCore().RadarSetttings.Settings.HiddenMarks;

        if (!string.IsNullOrEmpty(word) && GetCore().AvailableMarks.Contains(word))
        {
            if (hiddenMarks.Contains(word))
            {
                hiddenMarks.Remove(word);
                GetCore().RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.MarkerNowVisible", word.ColorLightBlue()));
            }
            else
            {
                hiddenMarks.Add(word);
                GetCore().RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.MarkerNowHidden", word.ColorLightBlue()));
            }
        }
        else
        {
            return TextCommandResult.Error(Lang.Get("mobsradar:Error.WordNotAvailable", word.ColorLightBlue()));
        }
    }

    private void PrintMarkerSizeColor(StringBuilder sb)
    {
        sb.AppendLine(Lang.Get("mobsradar:Radar.MarkersNameSizeColor"));
        var markers = GetCore().RadarSetttings.Settings.Markers;
        var sortedMarkers = markers.OrderBy(x => x.Key).ToList();

        var visibleMarks = GetCore().RadarSetttings.Settings.GetActiveMarks(_capi);
        var hiddenMarks = GetCore().RadarSetttings.Settings.HiddenMarks;

        int count = 0;
        foreach (var marker in sortedMarkers)
        {
            if (marker.Key != null && markers.ContainsKey(marker.Key))
            {
                var markColor = visibleMarks.Contains(marker.Key) ? "#4dffa6" : hiddenMarks.Contains(marker.Key) ? "#ff4d4d" : "";

                sb.Append(Lang.Get(
                    "mobsradar:Radar.MarkerNameSizeColorFormat",
                    marker.Key.ApplyColorToText(markColor),
                    markers[marker.Key].Size,
                    markers[marker.Key].Color.GetSelfColoredText()));

                count++;
                if (count % 3 == 0)
                    sb.AppendLine();
                else
                    sb.Append("\t\t");
            }
        }
    }

    private void InitializeTextures()
    {
        var worldMapManager = _capi.ModLoader.GetModSystem<WorldMapManager>();
        var mobsRadarMapLayer = (MobsRadarMapLayer)worldMapManager.MapLayers.Single(p => p is MobsRadarMapLayer);
        mobsRadarMapLayer.InitializeTextures();
    }
}
