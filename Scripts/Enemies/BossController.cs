using Godot;

public partial class BossController : Node2D
{
    private enum BossState
    {
        Invulnerable,
        Vulnerable,
    }

    [Export] public float InvulnerableDuration = 5f;
    [Export] public float VulnerableDuration = 3f;
    [Export] public float InvulnerableDrainMultiplier = 2.2f;

    private BossState _state;
    private float _stateTimer;
    private float _pulseTime;

    private PlayerController _player;
    private Sprite2D _bossSprite;
    private Area2D _coreArea;

    public override void _Ready()
    {
        _bossSprite = GetNodeOrNull<Sprite2D>("BossSprite");
        if (_bossSprite != null)
        {
            _bossSprite.Texture = AssetManager.Instance?.GetBossTexture();
            _bossSprite.RegionEnabled = true;
            _bossSprite.RegionRect = AssetManager.BossCollectorVisibleRegion;
            _bossSprite.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        }

        _coreArea = GetNodeOrNull<Area2D>("CoreArea");
        if (_coreArea != null)
        {
            _coreArea.BodyEntered += OnCoreBodyEntered;
        }

        EnterInvulnerable();
    }

    public override void _Process(double delta)
    {
        _stateTimer -= (float)delta;
        if (_stateTimer <= 0f)
        {
            if (_state == BossState.Invulnerable)
            {
                EnterVulnerable();
            }
            else
            {
                EnterInvulnerable();
            }
        }

        _pulseTime += (float)delta;
        float scale = 1f + (0.08f * Mathf.Sin(_pulseTime * 4f));
        Scale = new Vector2(scale, scale);

        if (_player == null)
        {
            _player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
            ApplyDrainMultiplier();
        }
    }

    public void AssignPlayer(PlayerController player)
    {
        _player = player;
        ApplyDrainMultiplier();
    }

    private void OnCoreBodyEntered(Node2D body)
    {
        if (_state != BossState.Vulnerable)
        {
            return;
        }

        if (body is not PlayerController)
        {
            return;
        }

        EnterInvulnerable();
    }

    private void EnterInvulnerable()
    {
        _state = BossState.Invulnerable;
        _stateTimer = InvulnerableDuration;
        if (_coreArea != null)
        {
            _coreArea.Monitoring = false;
        }

        Modulate = new Color(1f, 0.7f, 0.7f);
        ApplyDrainMultiplier();
    }

    private void EnterVulnerable()
    {
        _state = BossState.Vulnerable;
        _stateTimer = VulnerableDuration;
        if (_coreArea != null)
        {
            _coreArea.Monitoring = true;
        }

        Modulate = new Color(0.75f, 1f, 0.75f);
        ApplyDrainMultiplier();
    }

    private void ApplyDrainMultiplier()
    {
        if (_player == null)
        {
            return;
        }

        float multiplier = _state == BossState.Invulnerable ? InvulnerableDrainMultiplier : 1f;
        _player.SetDrainMultiplier(multiplier);
    }
}
