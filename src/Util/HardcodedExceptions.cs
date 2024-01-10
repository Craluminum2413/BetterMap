using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace MobsRadar;

public static class HardcodedExceptions
{
    public static string GetEntityConfigName(this ICoreClientAPI capi, Entity entity)
    {
        foreach ((string key, EntityMark value) in Core.Config.Markers)
        {
            if (entity.IsPet(capi.World.Player)) return "pet";
            if (entity.IsDead()) return "dead";
            if (entity.WildCardMatch(value.MatchTypes) || value.MatchClasses.Contains(entity.Class)) return key;
        }
        return "default";
    }

    public static bool IsDead(this Entity entity) => !entity.Alive;

    public static bool IsPet(this Entity entity, IClientPlayer player)
    {
        return entity.HasBehavior("tameable") && entity.WatchedAttributes.GetTreeAttribute("domesticationstatus")?.GetString("owner") == player.PlayerUID;
    }
}