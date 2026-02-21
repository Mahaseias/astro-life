using Godot;

public partial class GameSession : Node
{
    public const int CharacterMale = 0;
    public const int CharacterFemale = 1;

    public static GameSession Instance { get; private set; }

    [Export(PropertyHint.Enum, "Astronauta Masculino,Astronauta Feminina")]
    public int SelectedCharacter = CharacterMale;

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

    public void SelectMale()
    {
        SelectedCharacter = CharacterMale;
    }

    public void SelectFemale()
    {
        SelectedCharacter = CharacterFemale;
    }
}
