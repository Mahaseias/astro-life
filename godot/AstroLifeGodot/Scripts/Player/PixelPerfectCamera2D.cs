using Godot;

public partial class PixelPerfectCamera2D : Camera2D
{
    [Export] public bool SnapInProcess = true;
    [Export] public bool SnapInPhysics = false;

    public override void _Process(double delta)
    {
        if (SnapInProcess)
        {
            SnapToPixelGrid();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (SnapInPhysics)
        {
            SnapToPixelGrid();
        }
    }

    private void SnapToPixelGrid()
    {
        if (!Enabled)
        {
            return;
        }

        GlobalPosition = new Vector2(
            Mathf.Round(GlobalPosition.X),
            Mathf.Round(GlobalPosition.Y));
    }
}
