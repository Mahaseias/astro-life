using Godot;

[Tool]
public partial class AstroLifePipelinePlugin : EditorPlugin
{
    private const string MenuItem = "AstroLife/Build Pipeline";
    private const string TilesetMenuItem = "Astro/Build Tileset";

    public override void _EnterTree()
    {
        AddToolMenuItem(MenuItem, Callable.From(OnBuildPipelinePressed));
        AddToolMenuItem(TilesetMenuItem, Callable.From(OnBuildTilesetPressed));
    }

    public override void _ExitTree()
    {
        RemoveToolMenuItem(MenuItem);
        RemoveToolMenuItem(TilesetMenuItem);
    }

    private void OnBuildPipelinePressed()
    {
        GD.Print("[AstroLifePipelinePlugin] Building pipeline...");
        bool ok = AstroPipelineBuilder.BuildAll(EditorInterface.Singleton);
        if (ok)
        {
            GD.Print("[AstroLifePipelinePlugin] Build Pipeline finished.");
        }
        else
        {
            GD.PushWarning("[AstroLifePipelinePlugin] Build Pipeline finished with issues. Check Output.");
        }
    }

    private void OnBuildTilesetPressed()
    {
        GD.Print("[AstroLifePipelinePlugin] Building tileset...");
        var builder = new BuildAstroTileset();
        builder.Run();
    }
}
