using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace MobsRadar;

public static class HardcodedExceptions
{
    public static bool IsExcluded(this ICoreClientAPI capi, Entity entity) => entity is EntityPlayer { Player: not null }
        || entity is EntityTrader
        || entity.IsHidden()
        || IsOutOfRange(capi, capi.World.Player.Entity.Pos, entity.Pos);

    public static bool IsOutOfRange(this ICoreClientAPI capi, EntityPos pos1, EntityPos pos2)
    {
        var core = capi.ModLoader.GetModSystem<Core>();
        int horizontalRange = Math.Abs(pos2.XYZInt.X - pos1.XYZInt.X) + Math.Abs(pos2.XYZInt.Z - pos1.XYZInt.Z);
        int verticalRange = Math.Abs(pos2.XYZInt.Y - pos1.XYZInt.Y);
        return horizontalRange > core.GetHorizontalRadius() || verticalRange > core.GetVerticalRadius();
    }

    public static RadarMapComponent CreateMapComponentForProjectile(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForFish(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForBoat(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForBug(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForItem(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForHostile(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForPassive(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForNeutral(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);
    public static RadarMapComponent CreateMapComponentForDefault(this ICoreClientAPI capi, Entity entity, LoadedTexture texture) => new(capi, texture, entity);

    public static bool IsHidden(this Entity entity)
    {
        var hiddenMarks = entity.Api.ModLoader.GetModSystem<Core>().RadarSetttings.Settings.HiddenMarks;

        if (entity.IsProjectile() && hiddenMarks.Contains("projectile")) return true;
        else if (entity.IsFish() && hiddenMarks.Contains("fish")) return true;
        else if (entity.IsBoat() && hiddenMarks.Contains("boat")) return true;
        else if (entity.IsBug() && hiddenMarks.Contains("bugs")) return true;
        else if (entity.IsItem() && hiddenMarks.Contains("item")) return true;
        else if (entity.IsHostile() && hiddenMarks.Contains("hostile")) return true;
        else if (entity.IsPassive() && hiddenMarks.Contains("passive")) return true;
        else if (entity.IsNeutral() && hiddenMarks.Contains("neutral")) return true;
        else if (hiddenMarks.Contains("default")) return true;
        return false;
    }

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