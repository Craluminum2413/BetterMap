using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace MobsRadar;

public static class Textures
{
    public static LoadedTexture EnemyMarkTexture(this ICoreClientAPI capi, int size)
    {
        ImageSurface surface = new(Format.Argb32, size, size);
        Context ctx = new(surface);
        ctx.SetSourceRGBA(0, 0, 0, 0);
        ctx.Paint();
        capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, size, size, ColorUtil.Hex2Doubles("#000000"), ColorUtil.Hex2Doubles("#FF0000"));
        var enemyTexture = new LoadedTexture(capi, capi.Gui.LoadCairoTexture(surface, false), size / 2, size / 2);
        ctx.Dispose();
        surface.Dispose();
        return enemyTexture;
    }
}