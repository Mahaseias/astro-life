using Godot;

public partial class PlatformVisualController : Node2D
{
    [Export] public NodePath VisualPath = "Visual";
    [Export] public string AssetKey = AssetRegistry.PlatformMetalA;

    public override void _Ready()
    {
        Sprite2D visual = GetNodeOrNull<Sprite2D>(VisualPath);
        if (visual == null)
        {
            return;
        }

        if (AssetRegistry.Instance != null)
        {
            visual.Texture = AssetRegistry.Instance.GetTexture(AssetKey, visual.Texture);
            AssetRegistry.Instance.ApplyNearest(visual);
        }
        else
        {
            visual.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        }
    }
}
