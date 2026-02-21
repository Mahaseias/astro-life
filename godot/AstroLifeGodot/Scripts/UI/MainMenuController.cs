using Godot;

public partial class MainMenuController : Control
{
    public override void _Ready()
    {
        TextureRect background = GetNodeOrNull<TextureRect>("Background");
        Button playMaleButton = GetNode<Button>("Center/VBox/PlayMaleButton");
        Button playFemaleButton = GetNode<Button>("Center/VBox/PlayFemaleButton");
        Button quitButton = GetNode<Button>("Center/VBox/QuitButton");

        if (AssetRegistry.Instance != null)
        {
            if (background != null)
            {
                background.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.BackgroundSpace, background.Texture);
                AssetRegistry.Instance.ApplyNearest(background);
            }

            playMaleButton.Icon = AssetRegistry.Instance.GetTexture(AssetRegistry.PortraitMale, playMaleButton.Icon);
            playFemaleButton.Icon = AssetRegistry.Instance.GetTexture(AssetRegistry.PortraitFemale, playFemaleButton.Icon);
        }

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
