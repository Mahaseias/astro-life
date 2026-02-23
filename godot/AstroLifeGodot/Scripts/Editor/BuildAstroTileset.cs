using Godot;

[Tool]
public partial class BuildAstroTileset : EditorScript
{
    public void Run()
    {
        GD.Print("[AstroLife] BuildAstroTileset.Run()");
        BuildStationTileset.Build();
    }
}
