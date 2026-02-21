using Godot;

public partial class Checkpoint : Area2D
{
    [Export] public string CheckpointId = "checkpoint";
    [Export] public Color InactiveColor = new(0.3f, 0.8f, 1f, 1f);
    [Export] public Color ActiveColor = new(0.2f, 1f, 0.3f, 1f);

    private Polygon2D _visual;

    public override void _Ready()
    {
        _visual = GetNodeOrNull<Polygon2D>("Visual");
        BodyEntered += OnBodyEntered;
        SetVisual(false);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not PlayerController)
        {
            return;
        }

        RespawnManager.Instance?.SetCheckpoint(GlobalPosition, CheckpointId);
        SetVisual(true);
    }

    private void SetVisual(bool active)
    {
        if (_visual != null)
        {
            _visual.Color = active ? ActiveColor : InactiveColor;
        }
    }
}
