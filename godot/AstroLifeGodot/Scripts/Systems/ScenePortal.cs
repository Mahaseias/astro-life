using Godot;
using System.IO;

public partial class ScenePortal : Area2D
{
    [Export(PropertyHint.File, "*.tscn")] public string NextScenePath = "";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        Label label = GetNodeOrNull<Label>("Label");
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
