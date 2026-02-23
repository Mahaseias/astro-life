using Godot;

public partial class CharacterSelectController : Control
{
    [Export] public NodePath MascButtonPath = "Center/VBox/MascButton";
    [Export] public NodePath FemButtonPath = "Center/VBox/FemButton";
    [Export] public NodePath QuitButtonPath = "Center/VBox/QuitButton";

    public override void _Ready()
    {
        Button mascButton = GetNodeOrNull<Button>(MascButtonPath);
        Button femButton = GetNodeOrNull<Button>(FemButtonPath);
        Button quitButton = GetNodeOrNull<Button>(QuitButtonPath);

        if (mascButton != null)
        {
            mascButton.Icon = AssetManager.Instance?.CreateAtlasTexture(AssetManager.KeyAstroMasc, AssetManager.PlayerIdleRegion);
            mascButton.Pressed += () => StartGame(GameManager.CharacterOption.AstroMasc);
        }

        if (femButton != null)
        {
            femButton.Icon = AssetManager.Instance?.CreateAtlasTexture(AssetManager.KeyAstroFem, AssetManager.PlayerIdleRegion);
            femButton.Pressed += () => StartGame(GameManager.CharacterOption.AstroFem);
        }

        if (quitButton != null)
        {
            quitButton.Pressed += () => GetTree().Quit();
        }
    }

    private void StartGame(GameManager.CharacterOption selected)
    {
        GameManager.Instance?.SetSelectedCharacter(selected);
        GameManager.Instance?.ClearCheckpoint();

        Error error = GetTree().ChangeSceneToFile("res://Scenes/MainScene.tscn");
        if (error != Error.Ok)
        {
            GD.PushError($"Could not load main scene: {error}");
        }
    }
}
