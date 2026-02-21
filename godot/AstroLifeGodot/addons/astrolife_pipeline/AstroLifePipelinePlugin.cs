using Godot;

[Tool]
public partial class AstroLifePipelinePlugin : EditorPlugin
{
    private const string MenuItem = "AstroLife/Build Pipeline";

    public override void _EnterTree()
    {
        AddToolMenuItem(MenuItem, Callable.From(OnBuildPipelinePressed));
    }

    public override void _ExitTree()
    {
        RemoveToolMenuItem(MenuItem);
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
}
