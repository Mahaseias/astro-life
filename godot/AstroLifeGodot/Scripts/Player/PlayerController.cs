using Godot;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 260f;
    [Export] public float JumpVelocity = -520f;
    [Export] public float GravityScale = 1f;
    [Export] public NodePath OxygenSystemPath = "OxygenSystem";
    [Export] public NodePath CameraPath = "Camera2D";

    private float _gravity;
    private bool _jumpWasHeld;
    private OxygenSystem _oxygenSystem;

    public override void _Ready()
    {
        AddToGroup("player");

        _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity") * GravityScale;
        _oxygenSystem = GetNodeOrNull<OxygenSystem>(OxygenSystemPath);
        if (_oxygenSystem != null)
        {
            _oxygenSystem.OxygenEmpty += HandleOxygenEmpty;
        }

        Camera2D camera = GetNodeOrNull<Camera2D>(CameraPath);
        if (camera != null)
        {
            camera.Enabled = true;
            camera.PositionSmoothingEnabled = true;
            camera.PositionSmoothingSpeed = 5f;
        }

        RespawnManager.Instance?.RegisterPlayer(this);
        RespawnManager.Instance?.RegisterDefaultSpawn(GlobalPosition);
    }

    public override void _ExitTree()
    {
        if (_oxygenSystem != null)
        {
            _oxygenSystem.OxygenEmpty -= HandleOxygenEmpty;
        }

        RespawnManager.Instance?.UnregisterPlayer(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y += _gravity * (float)delta;
        }

        float horizontal = GetHorizontalInput();
        velocity.X = horizontal * MoveSpeed;

        bool jumpHeld = IsJumpHeld();
        bool jumpPressed = jumpHeld && !_jumpWasHeld;
        _jumpWasHeld = jumpHeld;

        if (jumpPressed && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void RespawnAt(Vector2 position)
    {
        GlobalPosition = position;
        Velocity = Vector2.Zero;
        _oxygenSystem?.ResetOxygen();
    }

    public void SetExternalDrainMultiplier(float multiplier)
    {
        _oxygenSystem?.SetExternalDrainMultiplier(multiplier);
    }

    public OxygenSystem GetOxygenSystem()
    {
        return _oxygenSystem;
    }

    private void HandleOxygenEmpty()
    {
        RespawnManager.Instance?.RequestRespawn();
    }

    private float GetHorizontalInput()
    {
        float horizontal = 0f;

        if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left))
        {
            horizontal -= 1f;
        }

        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right))
        {
            horizontal += 1f;
        }

        return Mathf.Clamp(horizontal, -1f, 1f);
    }

    private bool IsJumpHeld()
    {
        return Input.IsActionPressed("ui_accept") || Input.IsKeyPressed(Key.Space);
    }
}

