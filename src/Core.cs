using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

[assembly: ModInfo("Mobs Radar")]

namespace MobsRadar;

class Core : ModSystem
{
    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        var worldMapManager = api.ModLoader.GetModSystem<WorldMapManager>();

        worldMapManager.RegisterMapLayer<MobsRadarMapLayer>("Enemies");

        api.World.Logger.Event("started 'Mobs Radar' mod");
    }
}