using Godot;

public partial class StageArtAutoBinder : Node2D
{
    public override void _Ready()
    {
        if (AssetRegistry.Instance == null)
        {
            return;
        }

        ApplySprite("Background", AssetRegistry.BackgroundSpace);
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
}
