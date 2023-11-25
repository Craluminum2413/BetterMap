using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using System;
using Cairo;
using Vintagestory.API.MathTools;

namespace MobsRadar
{
    public class MobsRadarMapLayer : MapLayer
    {
        Dictionary<long, RadarMapComponent> MapComps = new Dictionary<long, RadarMapComponent>();
        ICoreClientAPI capi;

        Dictionary<EnumEntityCategory, LoadedEntityMark> loadedEntityMarkers;

        public override string Title => "Mobs Radar";
        public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

        public override string LayerGroupCode => "creatures";

        public MobsRadarMapLayer(ICoreAPI api, IWorldMapManager mapsink) : base(api, mapsink)
        {
            capi = api as ICoreClientAPI;
        }


        public override void OnLoaded()
        {
            if (capi != null)
            {
                // Only client side
                capi.Event.OnEntitySpawn += Event_OnEntitySpawn;
                capi.Event.OnEntityLoaded += Event_OnEntitySpawn;
                capi.Event.OnEntityDespawn += Event_OnEntityDespawn;
            }
        }

        private void Event_OnEntityDespawn(Entity entity, EntityDespawnData reasonData)
        {
            RadarMapComponent mp;
            if (MapComps.TryGetValue(entity.EntityId, out mp))
            {
                mp.Dispose();
                MapComps.Remove(entity.EntityId);
            }
        }

        private void Event_OnEntitySpawn(Entity entity)
        {
            if (entity is EntityPlayer) return;

            var loadedMarker = loadedEntityMarkers[EntityCategorizer.GetEntityCategory(entity, capi)];
            if (mapSink.IsOpened && !MapComps.ContainsKey(entity.EntityId) && loadedMarker.Visible)
            {
                RadarMapComponent cmp = new RadarMapComponent(capi, loadedMarker.texture, entity, entity.Properties.Color);
                MapComps[entity.EntityId] = cmp;
            }
        }



        public override void OnMapOpenedClient()
        {

            if (loadedEntityMarkers == null)
            {
                loadedEntityMarkers = new();
                var markerConfig = capi.ModLoader.GetModSystem<MobsRadar>().MarkerConfig;
                foreach (var category in Enum.GetValues<EnumEntityCategory>())
                {
                    var marker = markerConfig[category];
                    capi.Logger.Debug(string.Format("type: {0}, visibility: {1}", category, marker.Visible));
                    int size = (int)GuiElement.scaled(marker.Size);
                    ImageSurface surface = new ImageSurface(Format.Argb32, size, size);
                    Context ctx = new Context(surface);
                    ctx.SetSourceRGBA(0, 0, 0, 0);
                    ctx.Paint();
                    capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, size, size, new double[] { 0.3, 0.3, 0.3, 1 }, ColorUtil.Hex2Doubles(marker.Color));
                    loadedEntityMarkers.Add(category, new LoadedEntityMark()
                    {
                        Visible = marker.Visible,
                        texture = new LoadedTexture(capi, capi.Gui.LoadCairoTexture(surface, false), size / 2, size / 2)
                    });
                    ctx.Dispose();
                    surface.Dispose();
                }
            }



            foreach (var val in capi.World.LoadedEntities)
            {
                RadarMapComponent cmp;

                if (val.Value is EntityPlayer) continue;

                if (MapComps.TryGetValue(val.Value.EntityId, out cmp))
                {
                    cmp?.Dispose();
                    MapComps.Remove(val.Value.EntityId);
                }

                var loadedMarker = loadedEntityMarkers[EntityCategorizer.GetEntityCategory(val.Value, capi)];
                if (loadedMarker.Visible)
                {
                    cmp = new RadarMapComponent(capi, loadedMarker.texture, val.Value, val.Value.Properties.Color);
                    MapComps[val.Value.EntityId] = cmp;
                }
            }
        }


        public override void Render(GuiElementMap mapElem, float dt)
        {
            if (!Active) return;

            foreach (var val in MapComps)
            {
                val.Value.Render(mapElem, dt);
            }
        }

        public override void OnMouseMoveClient(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
        {
            if (!Active) return;

            foreach (var val in MapComps)
            {
                val.Value.OnMouseMove(args, mapElem, hoverText);
            }
        }

        public override void OnMouseUpClient(MouseEvent args, GuiElementMap mapElem)
        {
            if (!Active) return;

            foreach (var val in MapComps)
            {
                val.Value.OnMouseUpOnElement(args, mapElem);
            }
        }

        public override void OnMapClosedClient()
        {
            //Dispose();
            //MapComps.Clear();
        }


        public override void Dispose()
        {
            foreach (var val in MapComps)
            {
                val.Value?.Dispose();
            }
            foreach (var val in loadedEntityMarkers.Values)
            {
                val?.texture?.Dispose();
            }
            loadedEntityMarkers = null;
        }



    }
}
