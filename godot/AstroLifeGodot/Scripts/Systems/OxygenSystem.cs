using Godot;

public partial class OxygenSystem : Node
{
    [Signal] public delegate void OxygenChangedEventHandler(float current, float max);
    [Signal] public delegate void OxygenEmptyEventHandler();

    [Export] public float MaxOxygen = 100f;
    [Export] public float StartingOxygen = 100f;
    [Export] public float DrainPerSecond = 5f;
    [Export] public bool DrainEnabled = true;

    private float _current;
    private float _externalDrainMultiplier = 1f;
    private bool _emptySignalSent;

    public float CurrentOxygen => _current;

    public override void _Ready()
    {
        MaxOxygen = Mathf.Max(1f, MaxOxygen);
        _current = Mathf.Clamp(StartingOxygen, 0f, MaxOxygen);
        EmitOxygenChanged();
    }

    public override void _Process(double delta)
    {
        if (!DrainEnabled)
        {
            return;
        }

        Drain(DrainPerSecond * _externalDrainMultiplier * (float)delta);
    }

    public void SetExternalDrainMultiplier(float multiplier)
    {
        _externalDrainMultiplier = Mathf.Clamp(multiplier, 0f, 10f);
    }

    public void AddOxygen(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        SetOxygen(_current + amount);
    }

    public void Drain(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        SetOxygen(_current - amount);
    }

    public void ResetOxygen()
    {
        SetOxygen(MaxOxygen);
    }

    public void SetOxygen(float oxygen)
    {
        _current = Mathf.Clamp(oxygen, 0f, MaxOxygen);
        EmitOxygenChanged();

        if (_current <= 0f)
        {
            if (!_emptySignalSent)
            {
                _emptySignalSent = true;
                EmitSignal(SignalName.OxygenEmpty);
            }
        }
        else
        {
            _emptySignalSent = false;
        }
    }

    private void EmitOxygenChanged()
    {
        EmitSignal(SignalName.OxygenChanged, _current, MaxOxygen);
    }
}
