using Godot;

public partial class HudController : CanvasLayer
{
    [Export] public NodePath OxygenBarPath = "Root/OxygenBar";
    [Export] public NodePath OxygenTextPath = "Root/OxygenText";
    [Export] public NodePath DebugTextPath = "Root/DebugText";

    private ProgressBar _oxygenBar;
    private Label _oxygenText;
    private Label _debugText;

    private PlayerController _player;

    public override void _Ready()
    {
        _oxygenBar = GetNodeOrNull<ProgressBar>(OxygenBarPath);
        _oxygenText = GetNodeOrNull<Label>(OxygenTextPath);
        _debugText = GetNodeOrNull<Label>(DebugTextPath);
    }

    public override void _Process(double delta)
    {
        if (_player == null)
        {
            _player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
        }

        if (_player == null)
        {
            return;
        }

        OxygenSystem oxygen = _player.GetOxygenSystem();
        if (oxygen == null)
        {
            return;
        }

        if (_oxygenBar != null)
        {
            _oxygenBar.MaxValue = oxygen.MaxOxygen;
            _oxygenBar.Value = oxygen.CurrentOxygen;
        }

        if (_oxygenText != null)
        {
            _oxygenText.Text = $"Oxigenio: {oxygen.CurrentOxygen:0}/{oxygen.MaxOxygen:0}";
        }

        if (_debugText != null)
        {
            string sceneName = GetTree().CurrentScene != null ? GetTree().CurrentScene.Name : "none";
            string checkpoint = RespawnManager.Instance != null ? RespawnManager.Instance.GetActiveCheckpointLabel() : "spawn";
            _debugText.Text = $"Cena: {sceneName} | O2: {oxygen.CurrentOxygen:0} | CP: {checkpoint}";
        }
    }
}
