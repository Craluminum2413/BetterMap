using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace MobsRadar;

public class RadarHotkeys : ModSystem
{
    private ICoreClientAPI _capi;

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        _capi = api;

        api.Input.RegisterHotKey("mobsradar", Lang.Get("mobsradar:Action.ToggleRadar"), GlKeys.R, HotkeyType.GUIOrOtherControls, ctrlPressed: true);
        api.Input.SetHotKeyHandler("mobsradar", ToggleRadar);
    }

    private bool ToggleRadar(KeyCombination t1)
    {
        var core = _capi.ModLoader.GetModSystem<Core>();

        core.RadarSetttings.Settings.Enabled = !core.RadarSetttings.Settings.Enabled;
        core.RadarSetttings.Save();
        return true;
    }
}
