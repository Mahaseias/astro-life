using Godot;

public partial class HudController : CanvasLayer
{
    [Export] public NodePath OxygenBarPath = "Root/OxygenBar";
    [Export] public NodePath OxygenTextPath = "Root/OxygenText";
    [Export] public NodePath DebugTextPath = "Root/DebugText";
    [Export] public NodePath OxygenIconPath = "Root/OxygenIcon";
    [Export] public NodePath PanelPath = "Root/Panel";
    [Export] public bool AutoFixLegoPanel = true;
    [Export] public Color SolidPanelColor = new(0.10f, 0.16f, 0.28f, 0.78f);

    private TextureProgressBar _oxygenBar;
    private Label _oxygenText;
    private Label _debugText;
    private TextureRect _oxygenIcon;
    private TextureRect _panel;

    private PlayerController _player;

    public override void _Ready()
    {
        _oxygenBar = GetNodeOrNull<TextureProgressBar>(OxygenBarPath);
        _oxygenText = GetNodeOrNull<Label>(OxygenTextPath);
        _debugText = GetNodeOrNull<Label>(DebugTextPath);
        _oxygenIcon = GetNodeOrNull<TextureRect>(OxygenIconPath);
        _panel = GetNodeOrNull<TextureRect>(PanelPath);

        if (AssetRegistry.Instance != null)
        {
            if (_oxygenBar != null)
            {
                _oxygenBar.TextureUnder = AssetRegistry.Instance.GetTexture(AssetRegistry.UiOxygenBar, _oxygenBar.TextureUnder);
                _oxygenBar.TextureProgress = AssetRegistry.Instance.GetTexture(AssetRegistry.UiOxygenBarFill, _oxygenBar.TextureProgress);
                AssetRegistry.Instance.ApplyNearest(_oxygenBar);
            }

            if (_oxygenIcon != null)
            {
                _oxygenIcon.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.UiOxygenIcon, _oxygenIcon.Texture);
                AssetRegistry.Instance.ApplyNearest(_oxygenIcon);
            }

            if (_panel != null)
            {
                _panel.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.UiPanel, _panel.Texture);
                AssetRegistry.Instance.ApplyNearest(_panel);
            }
        }

        if (AutoFixLegoPanel)
        {
            ReplaceTiledPanelWithSolidBackdrop();
        }
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
            _oxygenText.Text = $"O2: {oxygen.CurrentOxygen:0}/{oxygen.MaxOxygen:0}";
        }

        if (_debugText != null)
        {
            string sceneName = GetTree().CurrentScene != null ? GetTree().CurrentScene.Name : "none";
            string checkpoint = RespawnManager.Instance != null ? RespawnManager.Instance.GetActiveCheckpointLabel() : "spawn";
            _debugText.Text = $"Scene: {sceneName} | O2: {oxygen.CurrentOxygen:0} | CP: {checkpoint}";
        }
    }

    private void ReplaceTiledPanelWithSolidBackdrop()
    {
        if (_panel == null)
        {
            return;
        }

        bool tiledPanel = _panel.StretchMode == TextureRect.StretchModeEnum.Tile;
        bool tinyTexture = _panel.Texture != null && (_panel.Texture.GetWidth() <= 32 || _panel.Texture.GetHeight() <= 32);
        if (!tiledPanel && !tinyTexture)
        {
            return;
        }

        Control root = _panel.GetParent<Control>();
        if (root == null)
        {
            return;
        }

        ColorRect backdrop = root.GetNodeOrNull<ColorRect>("HudBackdrop");
        if (backdrop == null)
        {
            backdrop = new ColorRect
            {
                Name = "HudBackdrop",
                Color = SolidPanelColor,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                OffsetLeft = _panel.OffsetLeft,
                OffsetTop = _panel.OffsetTop,
                OffsetRight = _panel.OffsetRight,
                OffsetBottom = _panel.OffsetBottom
            };

            root.AddChild(backdrop);
            backdrop.Owner = root.Owner;
            root.MoveChild(backdrop, _panel.GetIndex());
        }
        else
        {
            backdrop.Color = SolidPanelColor;
            backdrop.OffsetLeft = _panel.OffsetLeft;
            backdrop.OffsetTop = _panel.OffsetTop;
            backdrop.OffsetRight = _panel.OffsetRight;
            backdrop.OffsetBottom = _panel.OffsetBottom;
        }

        _panel.Visible = false;
    }
}
