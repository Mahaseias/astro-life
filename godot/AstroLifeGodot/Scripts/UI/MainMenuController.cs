using Godot;

public partial class MainMenuController : Control
{
    public override void _Ready()
    {
        Button playMaleButton = GetNode<Button>("Center/VBox/PlayMaleButton");
        Button playFemaleButton = GetNode<Button>("Center/VBox/PlayFemaleButton");
        Button quitButton = GetNode<Button>("Center/VBox/QuitButton");

        playMaleButton.Pressed += OnPlayMalePressed;
        playFemaleButton.Pressed += OnPlayFemalePressed;
        quitButton.Pressed += OnQuitPressed;
    }

    private void OnPlayMalePressed()
    {
        GameSession.Instance?.SelectMale();
        GetTree().ChangeSceneToFile(ScenePaths.W1_1);
    }

    private void OnPlayFemalePressed()
    {
        GameSession.Instance?.SelectFemale();
        GetTree().ChangeSceneToFile(ScenePaths.W1_1);
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
