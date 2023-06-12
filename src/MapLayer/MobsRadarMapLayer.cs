using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using System;

namespace MobsRadar
{
    public class MobsRadarMapLayer : MapLayer
    {
        private readonly Dictionary<Entity, EntityMapComponent> MapComps = new();
        private readonly ICoreClientAPI capi;
        private float secondsSinceLastTickUpdate;

        private Core GetCore() => capi.ModLoader.GetModSystem<Core>();

        private LoadedTexture fallbackTexture;       // Fallback texture for entities not found in EntityCodes
        private LoadedTexture itemTexture;           // Texture for items
        private LoadedTexture projectileTexture;     // Texture for projectiles
        private LoadedTexture fishTexture;           // Texture for fish
        private LoadedTexture bugTexture;            // Texture for butterflies
        private LoadedTexture boatTexture;           // Texture for boats

        private LoadedTexture hostileTexture;        // Texture for drifters
        private LoadedTexture passiveTexture;        // Texture for passive
        private LoadedTexture neutralTexture;        // Texture for neutral

        public override string Title => "MobsRadar";
        public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

        public MobsRadarMapLayer(ICoreAPI api, IWorldMapManager mapSink) : base(api, mapSink) => capi = api as ICoreClientAPI;

        public override void OnLoaded()
        {
            if (capi != null)
            {
                InitializeTextures();
                capi.Event.OnEntitySpawn += Event_EntitySpawn;
                capi.Event.OnEntityDespawn += Event_EntityDespawn;
            }
        }

        public override void OnTick(float dt)
        {
            if (!GetCore().RadarSetttings.Settings.Enabled) return;

            secondsSinceLastTickUpdate += dt;
            if (secondsSinceLastTickUpdate < 1) return;
            secondsSinceLastTickUpdate = 0;

            UpdateMarkers();
        }

        public override void Render(GuiElementMap mapElem, float dt)
        {
            if (!GetCore().RadarSetttings.Settings.Enabled) return;

            foreach (var val in MapComps)
            {
                if ((val.Key as EntityPlayer)?.Player != null || val.Key is EntityTrader)
                {
                    continue;
                }
                if (IsOutOfRange(capi.World.Player.Entity.Pos, val.Key.Pos))
                {
                    continue;
                }

                val.Value.Render(mapElem, dt);
            }
        }

        private bool IsOutOfRange(EntityPos pos1, EntityPos pos2)
        {
            int horizontalRange = Math.Abs(pos2.XYZInt.X - pos1.XYZInt.X) + Math.Abs(pos2.XYZInt.Z - pos1.XYZInt.Z);
            int verticalRange = Math.Abs(pos2.XYZInt.Y - pos1.XYZInt.Y);
            return horizontalRange > GetCore().GetHorizontalRadius() || verticalRange > GetCore().GetVerticalRadius();
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
            DisposeEntityMapComponents();
            DisposeTextures();
        }

        private void Event_EntitySpawn(Entity entity)
        {
            if (mapSink.IsOpened && !MapComps.ContainsKey(entity))
            {
                CreateEntityMapComponent(entity);
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

        private void InitializeTextures()
        {
            fallbackTexture = capi.DefaultMarkTexture();
            projectileTexture = capi.ProjectileMarkTexture();
            hostileTexture = capi.HostileMarkTexture();
            fishTexture = capi.FishMarkTexture();
            boatTexture = capi.BoatMarkTexture();
            bugTexture = capi.BugMarkTexture();
            itemTexture = capi.ItemMarkTexture();
            passiveTexture = capi.PassiveMarkTexture();
            neutralTexture = capi.NeutralMarkTexture();
        }

        private void UpdateMarkers()
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
                    capi.World.Logger.Warning("Can't add entity to world map, missing entity :<");
                    continue;
                }

                mapComponent = CreateEntityMapComponent(entity);
            }
        }

        private EntityMapComponent CreateEntityMapComponent(Entity entity)
        {
            EntityMapComponent mapComponent;

            switch (entity)
            {
                case EntityProjectile:
                case EntityThrownBeenade:
                case EntityThrownStone:
                    mapComponent = new EntityMapComponent(capi, projectileTexture, entity);
                    break;

                case EntityFish:
                    mapComponent = new EntityMapComponent(capi, fishTexture, entity);
                    break;

                default:
                    if (entity.Code.ToString().Contains("boat"))
                    {
                        mapComponent = new EntityMapComponent(capi, boatTexture, entity);
                    }
                    else if (entity.Code.ToString().Contains("butterfly") || entity.Code.ToString().Contains("grub"))
                    {
                        mapComponent = new EntityMapComponent(capi, bugTexture, entity);
                    }
                    else if (entity.Code.ToString().Contains("game:item"))
                    {
                        mapComponent = new EntityMapComponent(capi, itemTexture, entity);
                    }
                    else if (entity.Code.ToString().Contains("bee")
                        || entity.Code.ToString().Contains("eidolon")
                        || entity.Code.ToString().Contains("bee")
                        || entity.Code.ToString().Contains("bell")
                        || entity.Code.ToString().Contains("locust")
                        || entity.Code.ToString().Contains("drifter")
                        || entity.Code.ToString().Contains("wolf")
                        || entity.Code.ToString().Contains("hyena")
                        || entity.Code.ToString().Contains("bear"))
                    {
                        mapComponent = new EntityMapComponent(capi, hostileTexture, entity);
                    }
                    else if (entity.Code.ToString().Contains("hare")
                        || entity.Code.ToString().Contains("raccoon")
                        || entity.Code.ToString().Contains("gazelle"))
                    {
                        mapComponent = new EntityMapComponent(capi, passiveTexture, entity);
                    }
                    else if (entity.Code.ToString().Contains("fox")
                        || entity.Code.ToString().Contains("sheep")
                        || entity.Code.ToString().Contains("chicken")
                        || entity.Code.ToString().Contains("pig"))
                    {
                        mapComponent = new EntityMapComponent(capi, neutralTexture, entity);
                    }
                    else
                    {
                        mapComponent = new EntityMapComponent(capi, fallbackTexture, entity);
                    }
                    break;
            }

            MapComps[entity] = mapComponent;
            return mapComponent;
        }

        private void DisposeEntityMapComponents()
        {
            foreach (var val in MapComps)
            {
                val.Value?.Dispose();
            }

            MapComps.Clear();
        }

        private void DisposeTextures()
        {
            fallbackTexture?.Dispose();
            fallbackTexture = null;

            projectileTexture?.Dispose();
            projectileTexture = null;

            fishTexture?.Dispose();
            fishTexture = null;

            boatTexture?.Dispose();
            boatTexture = null;

            bugTexture?.Dispose();
            bugTexture = null;

            itemTexture?.Dispose();
            itemTexture = null;

            hostileTexture?.Dispose();
            hostileTexture = null;

            neutralTexture?.Dispose();
            neutralTexture = null;

            passiveTexture?.Dispose();
            passiveTexture = null;
        }
    }
}
