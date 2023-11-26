using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;

namespace MobsRadar;

public class LoadedEntityMark 
{
    public bool Visible { get; set; }
    public LoadedTexture texture { get; set; }
    public int maxHorizontalDistance {get; set;}
    public int maxVerticalDistance {get; set;}

    public bool ShouldBeRendered(Entity entity, ICoreClientAPI capi)
    {
        var player = capi.World.Player;
        var playerPos = player.Entity.Pos;
        var entityPos = entity.Pos;

        return Visible
            && playerPos.HorDistanceTo(entityPos) <= maxHorizontalDistance
            && Math.Abs(playerPos.Y - entityPos.Y) <= maxVerticalDistance;
    }
}
