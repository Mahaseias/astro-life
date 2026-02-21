using Godot;
using System.IO;

public partial class ScenePortal : Area2D
{
    [Export(PropertyHint.File, "*.tscn")] public string NextScenePath = "";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        Sprite2D visual = GetNodeOrNull<Sprite2D>("Visual");
        Label label = GetNodeOrNull<Label>("Label");

        if (visual != null)
        {
            if (AssetRegistry.Instance != null)
            {
                visual.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.Portal, visual.Texture);
                AssetRegistry.Instance.ApplyNearest(visual);
            }
            else
            {
                visual.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            }
        }

        if (label != null && !string.IsNullOrWhiteSpace(NextScenePath))
        {
            label.Text = Path.GetFileNameWithoutExtension(NextScenePath);
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not PlayerController)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(NextScenePath))
        {
            GD.PrintErr("ScenePortal sem NextScenePath definido.");
            return;
        }

        GetTree().ChangeSceneToFile(NextScenePath);
    }
}
