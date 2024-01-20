using ImGuiNET;
using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace MobsRadar;

public class EntityMark
{
    public bool Visible { get; set; }
    public string Icon { get; set; }
    public int Size { get; set; } = 28;
    public string Color { get; set; } = "#ffffff";
    public string[] MatchTypes { get; set; } = Array.Empty<string>();
    public string[] MatchClasses { get; set; } = Array.Empty<string>();

    public void Edit(string id)
    {
        bool visible = Visible;
        ImGui.Checkbox($"Visible##{id}", ref visible);
        Visible = visible;

        bool iconEnabled = Icon != null;
        ImGui.Checkbox($"Enable icon##{id}", ref iconEnabled);
        if (iconEnabled)
        {
            ImGui.TextWrapped(Config.CommentIcon);
            ImGui.Indent();
            AssetLocation icon = new(Icon ?? "game:textures/");
            VSImGui.AssetLocationEditor.Edit($"Icon##{id}", ref icon, icon);
            Icon = icon.ToString();
            ImGui.Unindent();
        }
        else
        {
            Icon = null;
        }
        

        int size = Size;
        ImGui.SliderInt($"Size##{id}", ref size, 1, 128);
        Size = size;

        double[] color4 = ColorUtil.Hex2Doubles(Color);
        System.Numerics.Vector3 color3 = new((float)color4[0], (float)color4[1], (float)color4[2]);
        ImGui.ColorEdit3($"Color##{id}", ref color3, ImGuiColorEditFlags.DisplayHex);
        Color = ColorUtil.Doubles2Hex(new double[] { color3[0], color3[1], color3[2] });

        string types = MatchTypes.Length > 0 ? MatchTypes.Aggregate((first, second) => $"{first}\n{second}") : "";
        ImGui.InputTextMultiline($"Types##{id}", ref types, 2048, new(0, 0));
        MatchTypes = types.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        string classes = MatchClasses.Length > 0 ? MatchClasses.Aggregate((first, second) => $"{first}\n{second}") : "";
        ImGui.InputTextMultiline($"Classes##{id}", ref classes, 2048, new(0, 0));
        MatchClasses = classes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }
}

public class LoadedEntityMark : EntityMark
{
    public LoadedTexture Texture { get; set; }

    public bool ShouldBeRendered(Entity entity, ICoreClientAPI capi)
    {
        IClientPlayer player = capi.World.Player;
        EntityPos playerPos = player.Entity.Pos;
        EntityPos entityPos = entity.Pos;

        return Visible
            && playerPos.HorDistanceTo(entityPos) <= Core.Config.HorizontalRadius
            && Math.Abs(playerPos.Y - entityPos.Y) <= Core.Config.VerticalRadius;
    }
}