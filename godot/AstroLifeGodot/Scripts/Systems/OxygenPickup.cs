using Godot;

public partial class OxygenPickup : Area2D
{
    [Export] public float OxygenAmount = 25f;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
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
