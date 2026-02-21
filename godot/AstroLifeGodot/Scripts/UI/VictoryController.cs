using Godot;

public partial class VictoryController : Control
{
    public override void _Ready()
    {
        Button backButton = GetNode<Button>("Center/VBox/BackButton");
        backButton.Pressed += OnBackPressed;
    }

    private void OnBackPressed()
    {
        GetTree().ChangeSceneToFile(ScenePaths.MainMenu);
    }
}
