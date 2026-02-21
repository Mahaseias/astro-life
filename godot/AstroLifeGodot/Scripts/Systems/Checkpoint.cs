using Godot;

public partial class Checkpoint : Area2D
{
    [Export] public string CheckpointId = "checkpoint";
    [Export] public NodePath BeaconPath = "Beacon";
    [Export] public NodePath HologramPath = "Hologram";

    private CanvasItem _beacon;
    private CanvasItem _hologram;
    private bool _isActive;
    private float _time;

    public override void _Ready()
    {
        _beacon = GetNodeOrNull<CanvasItem>(BeaconPath);
        _hologram = GetNodeOrNull<CanvasItem>(HologramPath);

        BodyEntered += OnBodyEntered;
        SetVisual(false);
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;

        if (_hologram != null)
        {
            float pulseSpeed = _isActive ? 5.5f : 2.2f;
            float alphaBase = _isActive ? 0.45f : 0.25f;
            float alphaRange = _isActive ? 0.4f : 0.15f;
            float alpha = alphaBase + ((Mathf.Sin(_time * pulseSpeed) + 1f) * 0.5f * alphaRange);
            _hologram.Modulate = new Color(1f, 1f, 1f, alpha);
        }

        if (_beacon != null && _isActive)
        {
            float warm = 0.88f + ((Mathf.Sin(_time * 6.2f) + 1f) * 0.06f);
            _beacon.Modulate = new Color(1f, warm, 0.95f, 1f);
        }
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
        _isActive = active;

        if (_beacon != null)
        {
            _beacon.Modulate = active
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(0.82f, 0.86f, 0.92f, 0.9f);
        }

        if (_hologram != null)
        {
            _hologram.Modulate = active
                ? new Color(1f, 1f, 1f, 0.6f)
                : new Color(1f, 1f, 1f, 0.25f);
        }
    }
}
