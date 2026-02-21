using Godot;

public partial class KillZone : Area2D
{
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
