using Godot;

public partial class StageArtAutoBinder : Node2D
{
    [Export] public Color ScreenBackdropColor = new(0.07f, 0.11f, 0.25f, 1f);

    public override void _Ready()
    {
        EnsureScreenBackdrop();

        if (AssetRegistry.Instance == null)
        {
            return;
        }

        ApplySprite("Background", GetBackgroundKeyForScene());
        ApplySprite("BackdropSatellite", AssetRegistry.PropSatellite);
        ApplySprite("BackdropSolar", AssetRegistry.PropSolarPanel);
        ApplySprite("BackdropSolarL", AssetRegistry.PropSolarPanel);
        ApplySprite("BackdropSolarR", AssetRegistry.PropSolarPanel);
        ApplySprite("BackdropModule", AssetRegistry.PropStationModule);
        ApplySprite("BackdropStation", AssetRegistry.PropStationModule);
        ApplySprite("HazardLaserL", AssetRegistry.HazardLaser);
        ApplySprite("HazardLaserR", AssetRegistry.HazardLaser);
        ApplySprite("HazardVaporL", AssetRegistry.HazardVapor);
        ApplySprite("HazardVaporR", AssetRegistry.HazardVapor);
    }

    private void ApplySprite(string nodeName, string assetKey)
    {
        Sprite2D sprite = GetNodeOrNull<Sprite2D>(nodeName);
        if (sprite == null)
        {
            return;
        }

        sprite.Texture = AssetRegistry.Instance.GetTexture(assetKey, sprite.Texture);
        AssetRegistry.Instance.ApplyNearest(sprite);
    }

    private void EnsureScreenBackdrop()
    {
        CanvasLayer layer = GetNodeOrNull<CanvasLayer>("__ScreenBackdropLayer");
        if (layer == null)
        {
            layer = new CanvasLayer
            {
                Name = "__ScreenBackdropLayer",
                Layer = -100
            };
            AddChild(layer);
            layer.Owner = this;
        }

        ColorRect rect = layer.GetNodeOrNull<ColorRect>("Backdrop");
        if (rect == null)
        {
            rect = new ColorRect
            {
                Name = "Backdrop",
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            rect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            layer.AddChild(rect);
            rect.Owner = this;
        }

        rect.Color = ScreenBackdropColor;
    }

    private string GetBackgroundKeyForScene()
    {
        string sceneName = Name.ToString();
        if (sceneName == "W1_1")
        {
            return AssetRegistry.BackgroundW11;
        }

        if (sceneName == "W1_2")
        {
            return AssetRegistry.BackgroundW12;
        }

        if (sceneName == "Boss")
        {
            return AssetRegistry.BackgroundBoss;
        }

        return AssetRegistry.BackgroundSpace;
    }
}
