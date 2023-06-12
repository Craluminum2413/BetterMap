using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

[assembly: ModInfo("Mobs Radar")]

namespace MobsRadar;

public class Core : ModSystem
{
    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        var worldMapManager = api.ModLoader.GetModSystem<WorldMapManager>();
        worldMapManager.RegisterMapLayer<MobsRadarMapLayer>("Enemies");

        api.Input.RegisterHotKey("mobsradar", Lang.Get("mobsradar:Show/Hide 'Mobs Radar'"), GlKeys.R, HotkeyType.GUIOrOtherControls, ctrlPressed: true);
        api.Input.SetHotKeyHandler("mobsradar", ToggleRadar);

        if (!ClientSettings.Inst.HasSetting("mRadarEnabled")) RadarSettings.Enabled = true;
        if (!ClientSettings.Inst.HasSetting("mRadarHorRadius")) RadarSettings.HorizontalRadius = 999;
        if (!ClientSettings.Inst.HasSetting("mRadarVerRadius")) RadarSettings.VerticalRadius = 20;

        var parsers = api.ChatCommands.Parsers;
        api.ChatCommands.Create("radar")
            .BeginSubCommand("hr")
                .WithArgs(parsers.Int("horizontal radius"))
                .HandleWith(x => ChangeRadius(x, EnumRadius.Horizontal))
            .EndSubCommand()
            .BeginSubCommand("vr")
                .WithArgs(parsers.Int("vertical radius"))
                .HandleWith(x => ChangeRadius(x, EnumRadius.Vertical))
            .EndSubCommand();

        api.World.Logger.Event("started 'Mobs Radar' mod");
    }

    private TextCommandResult ChangeRadius(TextCommandCallingArgs args, EnumRadius radius)
    {
        var num = (int)args[0];

        switch (radius)
        {
            case EnumRadius.Horizontal:
                RadarSettings.HorizontalRadius = num;
                return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Horizontal radius", num));
            case EnumRadius.Vertical:
                RadarSettings.VerticalRadius = num;
                return TextCommandResult.Success(Lang.Get("{0} set to {1}", "Vertical radius", num));
            default:
                return TextCommandResult.Deferred;
        }
    }

    private bool ToggleRadar(KeyCombination t1)
    {
        ClientSettings.Inst.Bool["mRadarEnabled"] = !RadarSettings.Enabled;
        return true;
    }
}

public enum EnumRadius
{
    Vertical,
    Horizontal
}

public static class RadarSettings
{
    public static bool Enabled
    {
        get { return ClientSettings.Inst.GetBoolSetting("mRadarEnabled"); }
        set { ClientSettings.Inst.Bool["mRadarEnabled"] = value; }
    }

    public static int HorizontalRadius
    {
        get { return ClientSettings.Inst.GetIntSetting("mRadarHorRadius"); }
        set { ClientSettings.Inst.Int["mRadarHorRadius"] = value; }
    }

    public static int VerticalRadius
    {
        get { return ClientSettings.Inst.GetIntSetting("mRadarVerRadius"); }
        set { ClientSettings.Inst.Int["mRadarVerRadius"] = value; }
    }
}