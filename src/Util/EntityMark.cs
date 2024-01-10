using System;

namespace MobsRadar;

public class EntityMark
{
    public bool Visible { get; set; }
    public string Icon { get; set; }
    public int Size { get; set; }
    public string Color { get; set; }
    public string[] MatchTypes { get; set; } = Array.Empty<string>();
    public string[] MatchClasses { get; set; } = Array.Empty<string>();
}
