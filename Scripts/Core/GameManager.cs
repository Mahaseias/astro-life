using Godot;

public partial class GameManager : Node
{
    public enum CharacterOption
    {
        AstroMasc,
        AstroFem,
    }

    public static GameManager Instance { get; private set; }

    [Export] public CharacterOption SelectedCharacter = CharacterOption.AstroMasc;

    public Vector2 CurrentCheckpoint { get; private set; } = new(160f, 260f);
    public bool HasCheckpoint { get; private set; }
    public bool SceneTransitionInProgress { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetSelectedCharacter(CharacterOption option)
    {
        SelectedCharacter = option;
    }

    public void SetCheckpoint(Vector2 checkpoint)
    {
        CurrentCheckpoint = checkpoint;
        HasCheckpoint = true;
    }

    public void ClearCheckpoint()
    {
        HasCheckpoint = false;
    }

    public Error SceneTransition(string scenePath)
    {
        if (SceneTransitionInProgress)
        {
            return Error.Busy;
        }

        SceneTransitionInProgress = true;
        Error result = GetTree().ChangeSceneToFile(scenePath);
        SceneTransitionInProgress = false;
        return result;
    }
}
