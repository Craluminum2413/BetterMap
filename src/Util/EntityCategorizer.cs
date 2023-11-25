using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace MobsRadar;

public static class EntityCategorizer
{
    public static EnumEntityCategory GetEntityCategory(Entity entity, ICoreClientAPI capi)
    {
        if (IsProjectile(entity)) return EnumEntityCategory.projectile;
        else if (IsFish(entity)) return EnumEntityCategory.fish;
        else if (IsBoat(entity)) return EnumEntityCategory.boat;
        else if (IsBug(entity)) return EnumEntityCategory.bugs;
        else if (IsItem(entity)) return EnumEntityCategory.item;
        else if (IsPet(entity, capi.World.Player)) return EnumEntityCategory.pet;
        else if (IsHostile(entity)) return EnumEntityCategory.hostile;
        else if (IsPassive(entity)) return EnumEntityCategory.passive;
        else if (IsNeutral(entity)) return EnumEntityCategory.neutral;
        else return EnumEntityCategory.other;
    }

    private static bool IsProjectile(Entity entity) => entity is EntityProjectile or EntityThrownBeenade or EntityThrownStone;
    private static bool IsFish(Entity entity) => entity is EntityFish;
    private static bool IsBoat(Entity entity) => entity.Code.ToString().Contains("boat") || entity is EntityBoat;
    private static bool IsBug(Entity entity) => entity.Code.ToString().Contains("butterfly") || entity.Code.ToString().Contains("grub");
    private static bool IsItem(Entity entity) => entity.Code.ToString().Contains("game:item");
    private static bool IsPet(Entity entity, IClientPlayer player) => entity.HasBehavior("tameable") && entity.WatchedAttributes.GetTreeAttribute("domesticationstatus")?.GetString("owner") == player.PlayerUID;

    private static bool IsHostile(Entity entity)
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

    private static bool IsPassive(Entity entity)
    {
        string code = entity.Code.ToString();
        return code.Contains("hare")
            || code.Contains("raccoon")
            || code.Contains("gazelle");
    }

    private static bool IsNeutral(Entity entity)
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