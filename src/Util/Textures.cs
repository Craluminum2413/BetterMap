using System;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace MobsRadar;

public static class Textures
{
    private static LoadedTexture CreateMarkTexture(this ICoreClientAPI capi, int size, string color)
    {
        var surface = new ImageSurface(Format.Argb32, size, size);
        var ctx = new Context(surface);

        ctx.SetSourceRGBA(0, 0, 0, 0);
        ctx.Paint();

        var opacity = GetSettings(capi).Opacity;

        switch (opacity)
        {
            case > 0:
                capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, size, size, ApplyOpacity("#000000", opacity), ApplyOpacity(color, opacity));
                break;
            default:
                capi.Gui.Icons.DrawMapPlayer(ctx, 0, 0, size, size, ColorUtil.Hex2Doubles("#000000"), ColorUtil.Hex2Doubles(color));
                break;
        }

        // Create a new surface and context for rotation
        var rotatedSurface = new ImageSurface(Format.Argb32, size, size);
        var rotatedCtx = new Context(rotatedSurface);

        // Rotate the original surface by 90 degrees
        rotatedCtx.Rotate(Math.PI / 2); // 90 degrees in radians
        rotatedCtx.SetSourceSurface(surface, 0, -size);
        rotatedCtx.Paint();

        return new LoadedTexture(capi, capi.Gui.LoadCairoTexture(rotatedSurface, false), size / 2, size / 2);
    }

    private static double[] ApplyOpacity(string color, double opacity) => new[]
    {
        ColorUtil.Hex2Doubles(color)[0],
        ColorUtil.Hex2Doubles(color)[1],
        ColorUtil.Hex2Doubles(color)[2],
        opacity
    };

    public static LoadedTexture MarkTexture(this ICoreClientAPI capi, EntityMark entityMark) => capi.CreateMarkTexture(entityMark.Size, entityMark.Color);

    public static LoadedTexture BoatMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["boat"].Size, GetSettings(capi).Markers["boat"].Color);
    public static LoadedTexture BugMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["bugs"].Size, GetSettings(capi).Markers["bugs"].Color);
    public static LoadedTexture DefaultMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["default"].Size, GetSettings(capi).Markers["default"].Color);
    public static LoadedTexture FishMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["fish"].Size, GetSettings(capi).Markers["fish"].Color);
    public static LoadedTexture HostileMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["hostile"].Size, GetSettings(capi).Markers["hostile"].Color);
    public static LoadedTexture ItemMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["item"].Size, GetSettings(capi).Markers["item"].Color);
    public static LoadedTexture NeutralMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["neutral"].Size, GetSettings(capi).Markers["neutral"].Color);
    public static LoadedTexture PassiveMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["passive"].Size, GetSettings(capi).Markers["passive"].Color);
    public static LoadedTexture ProjectileMarkTexture(this ICoreClientAPI capi) => capi.CreateMarkTexture(GetSettings(capi).Markers["projectile"].Size, GetSettings(capi).Markers["projectile"].Color);

    public static RadarSettings GetSettings(ICoreClientAPI capi) => capi.ModLoader.GetModSystem<Core>().RadarSetttings.Settings;
}