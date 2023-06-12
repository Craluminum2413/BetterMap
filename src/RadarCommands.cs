using System.Collections.Generic;
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
        .BeginSubCommand("marker")
            .WithArgs(parsers.Word("marker name to hide/show"))
            .HandleWith(HideOrShowMarker)
        .EndSubCommand();
    }

    private TextCommandResult PrintRadarInfo(TextCommandCallingArgs args)
    {
        var sb = new StringBuilder();

        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.HorizontalRadius", GetCore().RadarSetttings.Settings.HorizontalRadius.ToString()));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.VerticalRadius", GetCore().RadarSetttings.Settings.VerticalRadius.ToString()));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.Opacity", GetCore().RadarSetttings.Settings.Opacity.ToString()));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.AvailableMarks", string.Join(" ", GetCore().AvailableMarks)));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.VisibleMarks", string.Join(" ", GetCore().RadarSetttings.Settings.GetActiveMarks(_capi))));
        sb.AppendLine(Lang.Get("mobsradar:RadarInfo.HiddenMarks", string.Join(" ", GetCore().RadarSetttings.Settings.HiddenMarks)));

        return TextCommandResult.Success(sb.ToString());
    }

    private TextCommandResult SetRadius(TextCommandCallingArgs args, EnumRadius radius)
    {
        var num = (int)args[0];

        switch (radius)
        {
            case EnumRadius.Horizontal:
                GetCore().RadarSetttings.Settings.HorizontalRadius = num;
                GetCore().RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Horizontal radius", num));
            case EnumRadius.Vertical:
                GetCore().RadarSetttings.Settings.VerticalRadius = num;
                GetCore().RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Vertical radius", num));
            default:
                return TextCommandResult.Deferred;
        }
    }

    private TextCommandResult SetOpacity(TextCommandCallingArgs args)
    {
        var num = (float)args.LastArg;
        GetCore().RadarSetttings.Settings.Opacity = num;
        GetCore().RadarSetttings.Save();

        var worldMapManager = _capi.ModLoader.GetModSystem<WorldMapManager>();
        var mobsRadarMapLayer = (MobsRadarMapLayer)worldMapManager.MapLayers.Single(p => p is MobsRadarMapLayer);
        mobsRadarMapLayer.InitializeTextures();

        return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Opacity", num));
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
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.MarkerNowVisible", word));
            }
            else
            {
                hiddenMarks.Add(word);
                GetCore().RadarSetttings.Save();
                return TextCommandResult.Success(Lang.Get("mobsradar:Success.MarkerNowHidden", word));
            }
        }
        else
        {
            return TextCommandResult.Error(Lang.Get("mobsradar:Error.WordNotAvailable", word));
        }
    }
}
