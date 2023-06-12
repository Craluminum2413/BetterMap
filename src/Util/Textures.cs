using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace MobsRadar.Textures;

public static class Textures
{
    public class EntityMark
    {
        public int Size;
        public string Color; // hex
        public LoadedTexture Texture;
    }

    public static LoadedTexture CreateMarkTexture(this ICoreClientAPI capi, int size, string color)
    {
        var surface = new ImageSurface(Format.Argb32, size, size);
        var ctx = new Context(surface);

        ctx.SetSourceRGBA(0, 0, 0, 0);
        ctx.Paint();
        capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, size, size, ColorUtil.Hex2Doubles("#000000"), ColorUtil.Hex2Doubles(color));

        return new(capi, capi.Gui.LoadCairoTexture(surface, false), size / 2, size / 2);
    }

    public static LoadedTexture MarkTexture(this ICoreClientAPI capi, EntityMark entityMark) => capi.CreateMarkTexture(entityMark.Size, entityMark.Color);
    public static LoadedTexture DefaultMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(28, "#777777");

    public static LoadedTexture HostileMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(28, "#FF0000");
    public static LoadedTexture NeutralMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(28, "#ffa500");
    public static LoadedTexture PassiveMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(28, "#00FF00");

    public static LoadedTexture ProjectileMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(24, "#00FFFF");
    public static LoadedTexture FishMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(28, "#add8e6");
    public static LoadedTexture BoatMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(28, "#00AAFF");
    public static LoadedTexture BugMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(24, "#777777");
    public static LoadedTexture ItemMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(24, "#FF99FF");
}