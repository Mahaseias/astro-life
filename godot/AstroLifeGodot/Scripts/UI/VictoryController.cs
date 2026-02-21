using Godot;

public partial class VictoryController : Control
{
    public override void _Ready()
    {
        TextureRect background = GetNodeOrNull<TextureRect>("BackgroundTexture");
        Button backButton = GetNode<Button>("Center/VBox/BackButton");

        if (background != null && AssetRegistry.Instance != null)
        {
            background.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.BackgroundSpace, background.Texture);
            AssetRegistry.Instance.ApplyNearest(background);
        }

        backButton.Pressed += OnBackPressed;
    }

    private void OnBackPressed()
    {
        GetTree().ChangeSceneToFile(ScenePaths.MainMenu);
    }
}
