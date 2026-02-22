#if TOOLS
using Godot;

[Tool]
public partial class AstroBuildMenu : EditorPlugin
{
    public override void _EnterTree()
    {
        AddToolMenuItem("Astro/Build Tileset", Callable.From(Build));
    }

    public override void _ExitTree()
    {
        RemoveToolMenuItem("Astro/Build Tileset");
    }

    private void Build()
    {
        GD.Print("[AstroLife] Running BuildTileset...");

        var builder = new BuildAstroTileset();
        builder.Run();
    }
}
#endif
