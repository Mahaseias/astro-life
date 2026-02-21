using Godot;

public partial class MainMenuController : Control
{
    public override void _Ready()
    {
        Button playButton = GetNode<Button>("Center/VBox/PlayButton");
        Button quitButton = GetNode<Button>("Center/VBox/QuitButton");

        playButton.Pressed += OnPlayPressed;
        quitButton.Pressed += OnQuitPressed;
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile(ScenePaths.W1_1);
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
