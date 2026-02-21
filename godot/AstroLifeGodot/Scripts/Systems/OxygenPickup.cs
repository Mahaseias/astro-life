using Godot;

public partial class OxygenPickup : Area2D
{
    [Export] public float OxygenAmount = 25f;
    [Export] public float BobAmplitude = 2.5f;
    [Export] public float BobSpeed = 3.2f;
    [Export] public float GlowPulseSpeed = 4.8f;

    private Node2D _visual;
    private Node2D _glow;
    private Vector2 _visualBasePosition;
    private float _time;

    public override void _Ready()
    {
        _visual = GetNodeOrNull<Node2D>("Visual");
        _glow = GetNodeOrNull<Node2D>("Glow");
        _visualBasePosition = _visual != null ? _visual.Position : Vector2.Zero;

        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;

        if (_visual != null)
        {
            float bobY = Mathf.Sin(_time * BobSpeed) * BobAmplitude;
            _visual.Position = _visualBasePosition + new Vector2(0f, bobY);
        }

        if (_glow != null)
        {
            float pulse = 0.88f + (Mathf.Sin(_time * GlowPulseSpeed) + 1f) * 0.12f;
            _glow.Scale = new Vector2(pulse, pulse);

            if (_glow is CanvasItem glowItem)
            {
                float alpha = 0.45f + (Mathf.Sin(_time * GlowPulseSpeed) + 1f) * 0.2f;
                glowItem.Modulate = new Color(1f, 1f, 1f, alpha);
            }
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not PlayerController player)
        {
            return;
        }

        OxygenSystem oxygen = player.GetOxygenSystem();
        oxygen?.AddOxygen(OxygenAmount);
        QueueFree();
    }
}
