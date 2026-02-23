using Godot;

public partial class OxygenSystem : Node
{
    [Signal] public delegate void OxygenChangedEventHandler(float current, float max);
    [Signal] public delegate void OxygenEmptyEventHandler();

    [Export] public float MaxOxygen = 100f;
    [Export] public float StartingOxygen = 100f;
    [Export] public float BaseRate = 5f;
    [Export] public float Multiplier = 1f;

    private float _currentOxygen;
    private bool _emptySignalSent;

    public float CurrentOxygen => _currentOxygen;

    public override void _Ready()
    {
        MaxOxygen = Mathf.Max(1f, MaxOxygen);
        _currentOxygen = Mathf.Clamp(StartingOxygen, 0f, MaxOxygen);
        EmitSignal(SignalName.OxygenChanged, _currentOxygen, MaxOxygen);
    }

    public override void _Process(double delta)
    {
        float drainAmount = BaseRate * Multiplier * (float)delta;
        if (drainAmount > 0f)
        {
            Drain(drainAmount);
        }
    }

    public void SetMultiplier(float multiplier)
    {
        Multiplier = Mathf.Clamp(multiplier, 0f, 10f);
    }

    public void Add(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        SetOxygen(_currentOxygen + amount);
    }

    public void Drain(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        SetOxygen(_currentOxygen - amount);
    }

    public void ResetOxygen()
    {
        SetOxygen(MaxOxygen);
    }

    public void SetOxygen(float amount)
    {
        _currentOxygen = Mathf.Clamp(amount, 0f, MaxOxygen);
        EmitSignal(SignalName.OxygenChanged, _currentOxygen, MaxOxygen);

        if (_currentOxygen <= 0f)
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
}
