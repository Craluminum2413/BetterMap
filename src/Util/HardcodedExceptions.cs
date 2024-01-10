using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace MobsRadar;

public static class HardcodedExceptions
{
    // public static bool ShouldBeRendered(this ICoreClientAPI capi, Entity entity)
    // {
    //     return Core.Config.Markers.GetValueSafe(capi.GetEntityConfigName(entity)).Visible && !IsOutOfRange(capi, entity);
    // }

    // public static bool IsOutOfRange(this ICoreClientAPI capi, Entity entity)
    // {
    //     IClientPlayer player = capi.World.Player;
    //     EntityPos playerPos = player.Entity.Pos;
    //     EntityPos entityPos = entity.Pos;

    //     double horizontalRange = playerPos.HorDistanceTo(entityPos);
    //     double verticalRange = Math.Abs(playerPos.Y - entityPos.Y);
    //     return horizontalRange > Core.Config.HorizontalRadius || verticalRange > Core.Config.VerticalRadius;
    // }

    public static string GetEntityConfigName(this ICoreClientAPI capi, Entity entity)
    {
        if (entity.IsDead()) return "dead";
        else if (entity.IsPet(capi.World.Player)) return "pet";
        else if (entity.IsProjectile()) return "projectile";
        else if (entity.IsTrader()) return "trader";
        else if (entity.IsFish()) return "fish";
        else if (entity.IsBoat()) return "boat";
        else if (entity.IsBug()) return "bugs";
        else if (entity.IsItem()) return "item";
        else if (entity.IsHostile()) return "hostile";
        else if (entity.IsPassive()) return "passive";
        else if (entity.IsNeutral()) return "neutral";
        return "default";
    }

    public static bool IsDead(this Entity entity) => !entity.Alive;
    public static bool IsTrader(this Entity entity) => entity is EntityTrader;
    public static bool IsProjectile(this Entity entity) => entity is EntityProjectile or EntityThrownBeenade or EntityThrownStone;
    public static bool IsFish(this Entity entity) => entity is EntityFish;
    public static bool IsBoat(this Entity entity) => entity.Code.ToString().Contains("boat") || entity is EntityBoat;
    public static bool IsBug(this Entity entity) => entity.Code.ToString().Contains("butterfly") || entity.Code.ToString().Contains("grub");
    public static bool IsItem(this Entity entity) => entity.Code.ToString().Contains("game:item");
    public static bool IsPet(this Entity entity, IClientPlayer player) => entity.HasBehavior("tameable") && entity.WatchedAttributes.GetTreeAttribute("domesticationstatus")?.GetString("owner") == player.PlayerUID;

    public static bool IsHostile(this Entity entity)
    {
        string code = entity.Code.ToString();
        return code.Contains("bee")
            || code.Contains("eidolon")
            || code.Contains("bell")
            || code.Contains("locust")
            || code.Contains("drifter")
            || code.Contains("wolf")
            || code.Contains("hyena")
            || code.Contains("bear");
    }

    public static bool IsPassive(this Entity entity)
    {
        string code = entity.Code.ToString();
        return code.Contains("hare")
            || code.Contains("raccoon")
            || code.Contains("gazelle");
    }

    public static bool IsNeutral(this Entity entity)
    {
        string code = entity.Code.ToString();
        return code.Contains("fox")
            || code.Contains("sheep")
            || code.Contains("chicken")
            || code.Contains("pig")
            || code.Contains("deer")
            || code.Contains("moose");
    }
}