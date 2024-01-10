using System.Collections.Generic;
using System.Text;
using Cairo;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace MobsRadar;

public class MobsRadarMapLayer : MapLayer
{
    Dictionary<long, RadarMapComponent> MapComps = new();
    ICoreClientAPI capi;
    Dictionary<string, LoadedEntityMark> loadedEntityMarkers;
    float secondsSinceLastTickUpdate;

    public override string Title => "MobsRadar";
    public override string LayerGroupCode => "creatures";
    public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

    public MobsRadarMapLayer(ICoreAPI api, IWorldMapManager mapSink) : base(api, mapSink) => capi = api as ICoreClientAPI;

    public override void OnLoaded()
    {
        if (capi != null)
        {
            capi.Event.OnEntitySpawn += Event_EntitySpawn;
            capi.Event.OnEntityLoaded += Event_EntitySpawn;
            capi.Event.OnEntityDespawn += Event_EntityDespawn;
        }
    }

    private void Event_EntitySpawn(Entity entity)
    {
        if (entity is EntityPlayer) return;

        LoadedEntityMark loadedMarker = loadedEntityMarkers.GetValueSafe(capi.GetEntityConfigName(entity));
        if (mapSink.IsOpened && !MapComps.ContainsKey(entity.EntityId) && loadedMarker.ShouldBeRendered(entity, capi))
        {
            RadarMapComponent cmp = new RadarMapComponent(capi, loadedMarker.Texture, entity);
            MapComps[entity.EntityId] = cmp;
        }
    }

    private void Event_EntityDespawn(Entity entity, EntityDespawnData data)
    {
        if (MapComps.TryGetValue(entity.EntityId, out RadarMapComponent mapComponent))
        {
            mapComponent.Dispose();
            MapComps.Remove(entity.EntityId);
        }
    }

    public override void Render(GuiElementMap mapElem, float dt)
    {
        if (!Active) return;

        foreach (KeyValuePair<long, RadarMapComponent> val in MapComps)
        {
            val.Value.Render(mapElem, dt);
        }
    }

    public override void OnMouseMoveClient(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
    {
        if (!Active) return;

        foreach (KeyValuePair<long, RadarMapComponent> val in MapComps)
        {
            val.Value.OnMouseMove(args, mapElem, hoverText);
        }
    }

    public override void OnMouseUpClient(MouseEvent args, GuiElementMap mapElem)
    {
        if (!Active) return;

        foreach (KeyValuePair<long, RadarMapComponent> val in MapComps)
        {
            val.Value.OnMouseUpOnElement(args, mapElem);
        }
    }

    public override void Dispose()
    {
        foreach (KeyValuePair<long, RadarMapComponent> val in MapComps)
        {
            val.Value?.Dispose();
        }
        foreach (LoadedEntityMark val in loadedEntityMarkers.Values)
        {
            val?.Texture?.Dispose();
        }
        loadedEntityMarkers = null;
    }

    public override void OnTick(float dt)
    {
        if (!Active) return;

        secondsSinceLastTickUpdate += dt;
        if (secondsSinceLastTickUpdate < 1) return;
        secondsSinceLastTickUpdate = 0;

        UpdateTextures();
        UpdateMarkers();
    }

    public void UpdateTextures()
    {
        loadedEntityMarkers = new();
        foreach ((string key, EntityMark marker) in Core.Config.Markers)
        {
            int size = (int)GuiElement.scaled(marker.Size);
            ImageSurface surface = new ImageSurface(Format.Argb32, size, size);
            Context ctx = new Context(surface);
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();
            capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, size, size, new double[] { 0.3, 0.3, 0.3, 1 }, ColorUtil.Hex2Doubles(marker.Color));
            loadedEntityMarkers.Add(key, new LoadedEntityMark()
            {
                Visible = marker.Visible,
                Texture = new LoadedTexture(capi, capi.Gui.LoadCairoTexture(surface, false), size / 2, size / 2)
            });
            ctx.Dispose();
            surface.Dispose();
        }
    }

    public void UpdateMarkers()
    {
        foreach (KeyValuePair<long, Entity> val in capi.World.LoadedEntities)
        {
            RadarMapComponent cmp;

            if (val.Value is EntityPlayer) continue;

            if (MapComps.TryGetValue(val.Value.EntityId, out cmp))
            {
                cmp?.Dispose();
                MapComps.Remove(val.Value.EntityId);
            }

            LoadedEntityMark loadedMarker = loadedEntityMarkers[capi.GetEntityConfigName(val.Value)];
            if (loadedMarker.ShouldBeRendered(val.Value, capi))
            {
                cmp = new RadarMapComponent(capi, loadedMarker.Texture, val.Value);
                MapComps[val.Value.EntityId] = cmp;
            }
        }
    }
}
