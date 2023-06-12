using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

[assembly: ModInfo("Mobs Radar")]

namespace MobsRadar;

public class Core : ModSystem
{
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
            .WithArgs(parsers.Float("marker opacity"))
            .HandleWith(SetOpacity)
        // .EndSubCommand()
        // .BeginSubCommand("hide")
        //     .WithArgs(parsers.WordRange("markers blocklist"))
        //     .HandleWith(FillDisallowlist)
        // .EndSubCommand()
        // .BeginSubCommand("show")
        //     .WithArgs(parsers.WordRange("remove markers from blocklist"))
        //     .HandleWith(FillAllowlist)
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
        return TextCommandResult.Success(Lang.Get("mobsradar:Success.RadarInfo"));
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
        return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Opacity", num));
    }

    private TextCommandResult FillDisallowlist(TextCommandCallingArgs args)
    {
        var rawArgs = args.RawArgs;

        return TextCommandResult.Success(Lang.Get("mobsradar:Success.EntitiesNowHidden", rawArgs));
    }

    private TextCommandResult FillAllowlist(TextCommandCallingArgs args)
    {
        var rawArgs = args.RawArgs;

        return TextCommandResult.Success(Lang.Get("mobsradar:Success.EntitiesNowShown", rawArgs));
    }


    // private TextCommandResult FillAllowlist(TextCommandCallingArgs args)
    // {
    //     var rawArgs = args.RawArgs;

    //     return TextCommandResult.Success(Lang.Get("mobsradar:Success.EntitiesNowShown", rawArgs));
    // }
}
