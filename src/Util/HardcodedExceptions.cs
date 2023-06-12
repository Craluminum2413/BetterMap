using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace MobsRadar;

public static class HardcodedExceptions
{
    public static EntityMapComponent CreateMapComponentForProjectile(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForFish(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForBoat(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForBug(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForItem(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForHostile(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForPassive(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForNeutral(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static EntityMapComponent CreateMapComponentForDefault(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);

    public static bool IsProjectile(this Entity entity) => entity is EntityProjectile or EntityThrownBeenade or EntityThrownStone;
    public static bool IsFish(this Entity entity) => entity is EntityFish;
    public static bool IsBoat(this Entity entity) => entity.Code.ToString().Contains("boat") || entity is EntityBoat;
    public static bool IsBug(this Entity entity) => entity.Code.ToString().Contains("butterfly") || entity.Code.ToString().Contains("grub");
    public static bool IsItem(this Entity entity) => entity.Code.ToString().Contains("game:item");

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
            || code.Contains("pig");
    }
}