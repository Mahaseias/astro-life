using Godot;

public partial class KillZone : Area2D
{
    [Export] public bool ShowVisual = false;

    public override void _Ready()
    {
        Sprite2D visual = GetNodeOrNull<Sprite2D>("Visual");
        if (visual != null)
        {
            if (AssetRegistry.Instance != null)
            {
                visual.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.HazardKillZone, visual.Texture);
                AssetRegistry.Instance.ApplyNearest(visual);
            }
            else
            {
                visual.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            }

            visual.Visible = ShowVisual;
        }

        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController)
        {
            RespawnManager.Instance?.RequestRespawn();
        }
    }
}
