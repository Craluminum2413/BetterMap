using Vintagestory.API.Config;

namespace MobsRadar;

public static class TextExtensions
{
    public static string ApplyColorToText(this string text, string color)
    {
        if (string.IsNullOrEmpty(color))
        {
            return text;
        }
        else
        {
            return $"<font color=\"{color}\">{text}</font>";
        }
    }

    public static string GetSelfColoredText(this string color) => Lang.Get("mobsradar:SelfColoredText", color);
    public static string ColorLightBlue(this object text) => Lang.Get("mobsradar:Color.LightBlue", text.ToString());
}