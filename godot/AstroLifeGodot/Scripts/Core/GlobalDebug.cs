using Godot;

public partial class GlobalDebug : Node
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.Pressed || keyEvent.Echo)
        {
            return;
        }

        switch (keyEvent.Keycode)
        {
            case Key.F1:
                JumpToScene(ScenePaths.W1_1);
                break;
            case Key.F2:
                JumpToScene(ScenePaths.W1_2);
                break;
            case Key.F3:
                JumpToScene(ScenePaths.Boss);
                break;
            case Key.R:
                RespawnManager.Instance?.ForceRespawn();
                break;
        }
    }

    private void JumpToScene(string scenePath)
    {
        if (GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath == scenePath)
        {
            return;
        }

        GetTree().ChangeSceneToFile(scenePath);
    }
}
