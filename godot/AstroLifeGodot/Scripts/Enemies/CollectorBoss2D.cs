using Godot;

public partial class CollectorBoss2D : Node2D
{
    private enum BossState
    {
        Invulnerable,
        Vulnerable,
        Defeated
    }

    [Export] public int MaxHp = 3;
    [Export] public float InvulnerableDuration = 6f;
    [Export] public float VulnerableDuration = 4f;
    [Export] public float InvulnerableDrainMultiplier = 2.5f;

    private BossState _state;
    private int _hp;
    private float _stateTimer;

    private Area2D _coreArea;
    private Sprite2D _bodyVisual;
    private Sprite2D _coreVisual;
    private Label _statusLabel;
    private PlayerController _player;

    public override void _Ready()
    {
        _hp = Mathf.Max(1, MaxHp);

        _bodyVisual = GetNodeOrNull<Sprite2D>("Body");
        _coreArea = GetNode<Area2D>("Core");
        _coreVisual = GetNodeOrNull<Sprite2D>("Core/Visual");
        _statusLabel = GetNodeOrNull<Label>("StatusLabel");

        if (AssetRegistry.Instance != null)
        {
            if (_bodyVisual != null)
            {
                _bodyVisual.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.BossBody, _bodyVisual.Texture);
                AssetRegistry.Instance.ApplyNearest(_bodyVisual);
            }

            if (_coreVisual != null)
            {
                _coreVisual.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.BossCore, _coreVisual.Texture);
                AssetRegistry.Instance.ApplyNearest(_coreVisual);
            }
        }

        _coreArea.BodyEntered += OnCoreBodyEntered;
        EnterInvulnerable();
    }

    public override void _Process(double delta)
    {
        if (_state == BossState.Defeated)
        {
            return;
        }

        if (_player == null)
        {
            _player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
            ApplyDrainForCurrentState();
        }

        _stateTimer -= (float)delta;
        if (_stateTimer <= 0f)
        {
            if (_state == BossState.Invulnerable)
            {
                EnterVulnerable();
            }
            else if (_state == BossState.Vulnerable)
            {
                EnterInvulnerable();
            }
        }

        UpdateStatusText();
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

        _hp -= 1;
        if (_hp <= 0)
        {
            Defeat();
            return;
        }

        EnterInvulnerable();
    }

    private void EnterInvulnerable()
    {
        _state = BossState.Invulnerable;
        _stateTimer = InvulnerableDuration;
        _coreArea.Monitoring = false;
        Modulate = new Color(1f, 0.6f, 0.6f, 1f);
        ApplyDrainForCurrentState();
        UpdateStatusText();
    }

    private void EnterVulnerable()
    {
        _state = BossState.Vulnerable;
        _stateTimer = VulnerableDuration;
        _coreArea.Monitoring = true;
        Modulate = new Color(0.6f, 1f, 0.6f, 1f);
        ApplyDrainForCurrentState();
        UpdateStatusText();
    }

    private void Defeat()
    {
        _state = BossState.Defeated;
        _coreArea.Monitoring = false;
        Modulate = Colors.White;

        if (_player != null)
        {
            _player.SetExternalDrainMultiplier(1f);
        }

        UpdateStatusText("BOSS DERROTADO!");

        SceneTreeTimer timer = GetTree().CreateTimer(1.2f);
        timer.Timeout += () => { GetTree().ChangeSceneToFile(ScenePaths.Victory); };
    }

    private void ApplyDrainForCurrentState()
    {
        if (_player == null)
        {
            return;
        }

        if (_state == BossState.Invulnerable)
        {
            _player.SetExternalDrainMultiplier(InvulnerableDrainMultiplier);
        }
        else
        {
            _player.SetExternalDrainMultiplier(1f);
        }
    }

    private void UpdateStatusText(string overrideText = "")
    {
        if (_statusLabel == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(overrideText))
        {
            _statusLabel.Text = overrideText;
            return;
        }

        string stateText = _state == BossState.Invulnerable ? "INVULNERAVEL" :
            _state == BossState.Vulnerable ? "VULNERAVEL" : "DERROTADO";

        _statusLabel.Text = $"Boss HP: {_hp} | {stateText}";
    }
}
