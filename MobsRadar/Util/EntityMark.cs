using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;

namespace MobsRadar;

public class EntityMark
{
    public bool Visible { get; set; } = true;
    public string Icon { get; set; }
    public int Size { get; set; } = 28;
    public string Color { get; set; } = "#FFFFFF";
    public string[] MatchTypes { get; set; } = Array.Empty<string>();
    public string[] MatchClasses { get; set; } = Array.Empty<string>();
}

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
