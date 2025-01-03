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

        if (loadedEntityMarkers == null || loadedEntityMarkers.Count == 0)
        {
            return;
        }

        LoadedEntityMark loadedMarker = loadedEntityMarkers.GetValueSafe(capi.GetEntityConfigName(entity));

        if (loadedMarker == null)
        {
            return;
        }

        if (!mapSink.IsOpened || MapComps.ContainsKey(entity.EntityId) || !loadedMarker.ShouldBeRendered(entity, capi))
        {
            return;
        }

        RadarMapComponent cmp = new RadarMapComponent(capi, loadedMarker.Texture, entity);
        MapComps[entity.EntityId] = cmp;
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

        if (loadedEntityMarkers == null || loadedEntityMarkers.Count == 0)
        {
            return;
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

        if (Core.Config == null)
        {
            return;
        }

        if (Core.Config.RefreshRate == -1)
        {
            return;
        }

        secondsSinceLastTickUpdate += dt;
        if (secondsSinceLastTickUpdate < (Core.Config.RefreshRate / 1000)) return;
        secondsSinceLastTickUpdate = 0;

        Dispose();
        UpdateTextures();
        UpdateMarkers();
    }

    public void UpdateTextures()
    {
        loadedEntityMarkers = new();
        foreach ((string key, EntityMark marker) in Core.Config.Markers)
        {
            int size = (int)GuiElement.scaled(marker.Size);

            loadedEntityMarkers.Add(key, new LoadedEntityMark()
            {
                Visible = marker.Visible,
                Texture = DrawIcon(marker, size)
            });
        }
    }

    private LoadedTexture DrawIcon(EntityMark marker, int size)
    {
        if (!string.IsNullOrEmpty(marker.Icon) && api.Assets.TryGet(marker.Icon) != null)
        {
            return capi.Gui.LoadSvgWithPadding(new AssetLocation(marker.Icon), size, size, 7, marker.Color != null ? ColorUtil.Hex2Int(marker.Color) : null);
        }
        else
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, size, size);
            Context ctx = new Context(surface);
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();
            capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, size, size, new double[] { 0.3, 0.3, 0.3, 1 }, ColorUtil.Hex2Doubles(marker.Color));

            LoadedTexture _texture = new LoadedTexture(capi, capi.Gui.LoadCairoTexture(surface, false), size / 2, size / 2);
            ctx.Dispose();
            surface.Dispose();
            return _texture;
        }
    }

    public void UpdateMarkers()
    {
        foreach ((long id, Entity entity) in capi.World.LoadedEntities)
        {
            RadarMapComponent cmp;

            if (entity is EntityPlayer) continue;

            if (MapComps.TryGetValue(entity.EntityId, out cmp))
            {
                cmp?.Dispose();
                MapComps.Remove(entity.EntityId);
            }

            LoadedEntityMark loadedMarker = loadedEntityMarkers.GetValueSafe(capi.GetEntityConfigName(entity));
            if (loadedMarker == null || !loadedMarker.ShouldBeRendered(entity, capi))
            {
                continue;
            }

            cmp = new RadarMapComponent(capi, loadedMarker.Texture, entity);
            MapComps[entity.EntityId] = cmp;
        }
    }
}
