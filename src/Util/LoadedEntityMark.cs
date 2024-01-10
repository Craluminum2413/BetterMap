using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;

namespace MobsRadar;

public class LoadedEntityMark : EntityMark
{
    public LoadedTexture Texture { get; set; }

    public bool ShouldBeRendered(Entity entity, ICoreClientAPI capi)
    {
        IClientPlayer player = capi.World.Player;
        EntityPos playerPos = player.Entity.Pos;
        EntityPos entityPos = entity.Pos;

        return Visible
            && playerPos.HorDistanceTo(entityPos) <= Core.Config.HorizontalRadius
            && Math.Abs(playerPos.Y - entityPos.Y) <= Core.Config.VerticalRadius;
    }
}
