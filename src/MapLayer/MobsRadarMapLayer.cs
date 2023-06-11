using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace MobsRadar;

public class MobsRadarMapLayer : MapLayer
{
    public Dictionary<Entity, EntityMapComponent> MapComps = new();
    public ICoreClientAPI capi;
    float secondsSinceLastTickUpdate;

    readonly Dictionary<string, LoadedTexture> MapTextures = new() { { "Enemy", default } };

    public override string Title => "MobsRadar";
    public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

    public MobsRadarMapLayer(ICoreAPI api, IWorldMapManager mapSink) : base(api, mapSink) => capi = api as ICoreClientAPI;

    private void Event_EntitySpawn(Entity entity)
    {
        // TO-DO: Show markers only for entities that can deal damage
        // AiTaskManager taskManager = entity?.GetBehavior<EntityBehaviorTaskAI>()?.TaskManager;
        // if (taskManager?.GetTask<AiTaskMeleeAttack>() != null)
        // {
        //     return;
        // }

        if (mapSink.IsOpened && !MapComps.ContainsKey(entity))
        {
            MapComps[entity] = new EntityMapComponent(capi, MapTextures["Enemy"], entity);
        }
    }

    private void Event_EntityDespawn(Entity entity, EntityDespawnData data)
    {
        if (MapComps.TryGetValue(entity, out var mapComponent))
        {
            mapComponent.Dispose();
            MapComps.Remove(entity);
        }
    }

    public override void OnLoaded()
    {
        if (capi != null)
        {
            int size = (int)GuiElement.scaled(32);
            MapTextures["Enemy"] ??= capi.EnemyMarkTexture(size);

            capi.Event.OnEntitySpawn += Event_EntitySpawn;
            capi.Event.OnEntityDespawn += Event_EntityDespawn;
        }
    }

    public override void OnTick(float dt)
    {
        secondsSinceLastTickUpdate += dt;
        if (secondsSinceLastTickUpdate < 1) return;
        secondsSinceLastTickUpdate = 0;

        UpdateEnemyMarkers();
    }

    private void UpdateEnemyMarkers()
    {
        foreach (var entity in capi.World.LoadedEntities.Values)
        {
            if (MapComps.TryGetValue(entity, out var mapComponent))
            {
                mapComponent?.Dispose();
                MapComps.Remove(entity);
            }

            if (entity == null)
            {
                capi.World.Logger.Warning("Can't add enemy entity to world map, missing entity :<");
                continue;
            }

            mapComponent = new EntityMapComponent(capi, MapTextures["Enemy"], entity);

            MapComps[entity] = mapComponent;
        }
    }

    public override void Render(GuiElementMap mapElem, float dt)
    {
        foreach (var val in MapComps)
        {
            if ((val.Key as EntityPlayer)?.Player != null || val.Key is EntityThrownStone)
            {
                continue;
            }

            val.Value.Render(mapElem, dt);
        }
    }

    public override void OnMouseMoveClient(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
    {
        foreach (var val in MapComps)
        {
            val.Value.OnMouseMove(args, mapElem, hoverText);
        }
    }

    public override void OnMouseUpClient(MouseEvent args, GuiElementMap mapElem)
    {
        foreach (var val in MapComps)
        {
            val.Value.OnMouseUpOnElement(args, mapElem);
        }
    }

    public override void OnMapClosedClient() { }

    public override void Dispose()
    {
        foreach (var val in MapComps)
        {
            val.Value?.Dispose();
        }

        MapTextures["Enemy"]?.Dispose();
        MapTextures["Enemy"] = null;
    }
}