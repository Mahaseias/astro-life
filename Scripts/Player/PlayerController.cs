using Godot;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 220f;
    [Export] public float JumpVelocity = -360f;
    [Export] public float GravityScale = 1f;
    [Export] public float RunDrainMultiplier = 1.25f;

    private float _gravity;
    private Vector2 _spawnPoint;
    private float _bossDrainMultiplier = 1f;

    private AnimatedSprite2D _animatedSprite;
    private OxygenSystem _oxygenSystem;

    public override void _Ready()
    {
        AddToGroup("player");

        _spawnPoint = GlobalPosition;
        _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity") * GravityScale;

        _animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        if (_animatedSprite != null && (_animatedSprite.SpriteFrames == null || !_animatedSprite.SpriteFrames.HasAnimation("idle")))
        {
            GameManager.CharacterOption option = GameManager.Instance?.SelectedCharacter ?? GameManager.CharacterOption.AstroMasc;
            _animatedSprite.SpriteFrames = AssetManager.Instance?.BuildPlayerFrames(option);
        }

        if (_animatedSprite != null)
        {
            _animatedSprite.Play("idle");
        }

        if (_oxygenSystem == null)
        {
            OxygenSystem siblingOxygen = GetParent().GetNodeOrNull<OxygenSystem>("OxygenSystem");
            BindOxygenSystem(siblingOxygen);
        }
    }

    public override void _ExitTree()
    {
        if (_oxygenSystem != null)
        {
            _oxygenSystem.OxygenEmpty -= OnOxygenEmpty;
        }
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

        if (CanJump())
        {
            velocity.Y = JumpVelocity;
        }

        Velocity = velocity;
        MoveAndSlide();

        UpdateAnimation(horizontal);
        UpdateDrainMultiplier(horizontal);
    }

    public void BindOxygenSystem(OxygenSystem oxygenSystem)
    {
        if (_oxygenSystem != null)
        {
            _oxygenSystem.OxygenEmpty -= OnOxygenEmpty;
        }

        _oxygenSystem = oxygenSystem;

        if (_oxygenSystem != null)
        {
            _oxygenSystem.OxygenEmpty += OnOxygenEmpty;
        }
    }

    public void SetDrainMultiplier(float multiplier)
    {
        _bossDrainMultiplier = Mathf.Clamp(multiplier, 0f, 10f);
    }

    public OxygenSystem GetOxygenSystem()
    {
        return _oxygenSystem;
    }

    private float GetHorizontalInput()
    {
        float horizontal = 0f;

        if (Input.IsActionPressed("ui_left") || Input.IsPhysicalKeyPressed(Key.A) || Input.IsPhysicalKeyPressed(Key.Left))
        {
            horizontal -= 1f;
        }

        if (Input.IsActionPressed("ui_right") || Input.IsPhysicalKeyPressed(Key.D) || Input.IsPhysicalKeyPressed(Key.Right))
        {
            horizontal += 1f;
        }

        return Mathf.Clamp(horizontal, -1f, 1f);
    }

    private bool CanJump()
    {
        return IsOnFloor() && (Input.IsActionJustPressed("ui_accept") || Input.IsPhysicalKeyPressed(Key.Space));
    }

    private void UpdateAnimation(float horizontal)
    {
        if (_animatedSprite == null)
        {
            return;
        }

        if (!IsOnFloor() && _animatedSprite.SpriteFrames != null && _animatedSprite.SpriteFrames.HasAnimation("jump"))
        {
            _animatedSprite.Play("jump");
            return;
        }

        if (Mathf.Abs(horizontal) > 0.05f)
        {
            _animatedSprite.FlipH = horizontal < 0f;
            _animatedSprite.Play("run");
        }
        else
        {
            _animatedSprite.Play("idle");
        }
    }

    private void UpdateDrainMultiplier(float horizontal)
    {
        if (_oxygenSystem == null)
        {
            return;
        }

        float movementMultiplier = Mathf.Abs(horizontal) > 0.05f ? RunDrainMultiplier : 1f;
        _oxygenSystem.SetMultiplier(_bossDrainMultiplier * movementMultiplier);
    }

    private void OnOxygenEmpty()
    {
        Vector2 respawnPoint = _spawnPoint;
        if (GameManager.Instance != null && GameManager.Instance.HasCheckpoint)
        {
            respawnPoint = GameManager.Instance.CurrentCheckpoint;
        }

        GlobalPosition = respawnPoint;
        Velocity = Vector2.Zero;
        _oxygenSystem?.ResetOxygen();
    }
}
