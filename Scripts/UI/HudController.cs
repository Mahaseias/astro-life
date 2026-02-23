using Godot;

public partial class HudController : CanvasLayer
{
    [Export] public NodePath OxygenBarPath = "Root/OxygenBar";
    [Export] public NodePath OxygenLabelPath = "Root/OxygenLabel";

    private TextureProgressBar _oxygenBar;
    private Label _oxygenLabel;
    private OxygenSystem _oxygenSystem;

    public override void _Ready()
    {
        _oxygenBar = GetNodeOrNull<TextureProgressBar>(OxygenBarPath);
        _oxygenLabel = GetNodeOrNull<Label>(OxygenLabelPath);
    }

    public void BindOxygenSystem(OxygenSystem oxygenSystem)
    {
        if (_oxygenSystem != null)
        {
            _oxygenSystem.OxygenChanged -= OnOxygenChanged;
            _oxygenSystem.OxygenEmpty -= OnOxygenEmpty;
        }

        _oxygenSystem = oxygenSystem;

        if (_oxygenSystem != null)
        {
            _oxygenSystem.OxygenChanged += OnOxygenChanged;
            _oxygenSystem.OxygenEmpty += OnOxygenEmpty;
            OnOxygenChanged(_oxygenSystem.CurrentOxygen, _oxygenSystem.MaxOxygen);
        }
    }

    private void OnOxygenChanged(float current, float max)
    {
        if (_oxygenBar != null)
        {
            _oxygenBar.MaxValue = max;
            _oxygenBar.Value = current;
        }

        if (_oxygenLabel != null)
        {
            _oxygenLabel.Text = $"O2: {current:0}/{max:0}";
        }
    }

    private void OnOxygenEmpty()
    {
        if (_oxygenLabel != null)
        {
            _oxygenLabel.Text = "O2: 0";
        }
    }
}
