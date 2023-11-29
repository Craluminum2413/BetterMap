using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace MobsRadar;

public class RadarMapComponent : EntityMapComponent
{
    public RadarMapComponent(ICoreClientAPI capi, LoadedTexture texture, Entity entity, string color = null) : base(capi, texture, entity, color)
    {
    }

    public override void OnMouseMove(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
    {
        var playerEntity = capi.World.Player.Entity;
        var viewPos = new Vec2f();

        mapElem.TranslateWorldPosToViewPos(entity.Pos.XYZ, ref viewPos);
        double mouseX = args.X - mapElem.Bounds.renderX;
        double mouseY = args.Y - mapElem.Bounds.renderY;
        if (Math.Abs(viewPos.X - mouseX) < 5.0 && Math.Abs(viewPos.Y - mouseY) < 5.0)
        {
            double distance = playerEntity.Pos.DistanceTo(entity.Pos);

            hoverText.Append(GetEntityName(entity));

            if (entity.WatchedAttributes.HasAttribute("health"))
            {
                var healthTree = entity.WatchedAttributes.GetTreeAttribute("health");
                float currentHealth = healthTree.GetFloat("currenthealth");
                float maxHealth = healthTree.GetFloat("maxhealth");
                hoverText.Append(" | ").Append(Lang.Get("Health: {0}/{1}", currentHealth, maxHealth));
            }

            hoverText.Append(" | ").AppendLine(Lang.Get("createworld-worldheight", Math.Round(distance).ToString()));
        }
    }

    private string GetEntityName(Entity entity)
    {
        string original = entity.GetName();

        return entity switch
        {
            EntityItem entityItem => entityItem?.Slot?.GetStackName(),
            EntityBlockFalling entityBlockFalling => entityBlockFalling?.Block?.GetPlacedBlockName(entity.World, entity.Pos.AsBlockPos),
            _ => Lang.GetMatching(original.Replace("-creature", "").Replace("thrownstone", "stone")),
        };
    }
}
