using Godot;

[Tool]
public partial class TestLevelAtlasBinder : Node2D
{
    [Export] public NodePath RegistryPath = "AtlasRegistry";
    [Export] public NodePath TileMapPath = "TileMap";
    [Export] public NodePath PickupPath = "OxygenPickupSprite";
    [Export] public NodePath CheckpointPath = "CheckpointSprite";
    [Export] public NodePath UiBarPath = "UI/Root/OxygenBarPreview";

    private AtlasRegistry _registry;

    public override void _Ready()
    {
        _registry = GetNodeOrNull<AtlasRegistry>(RegistryPath);
        ApplyAtlasTextures();
    }

    private void ApplyAtlasTextures()
    {
        if (_registry == null)
        {
            GD.PushWarning("[TestLevelAtlasBinder] AtlasRegistry node not found.");
        }

        TileMapLayer tileMap = GetNodeOrNull<TileMapLayer>(TileMapPath);
        if (tileMap != null)
        {
            EnsureSampleTileLayout(tileMap);
            tileMap.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            tileMap.TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled;
        }

        Sprite2D pickup = GetNodeOrNull<Sprite2D>(PickupPath);
        if (pickup != null)
        {
            if (_registry != null)
            {
                pickup.Texture = _registry.Get("OxygenPickup") ?? pickup.Texture;
            }

            pickup.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            pickup.TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled;
        }

        Sprite2D checkpoint = GetNodeOrNull<Sprite2D>(CheckpointPath);
        if (checkpoint != null)
        {
            if (_registry != null)
            {
                checkpoint.Texture = _registry.Get("Checkpoint") ?? checkpoint.Texture;
            }

            checkpoint.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            checkpoint.TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled;
        }

        TextureRect oxygenBar = GetNodeOrNull<TextureRect>(UiBarPath);
        if (oxygenBar != null)
        {
            if (_registry != null)
            {
                oxygenBar.Texture = _registry.Get("UI_OxygenBar") ?? oxygenBar.Texture;
            }

            oxygenBar.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            oxygenBar.TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled;
        }
    }

    private static void EnsureSampleTileLayout(TileMapLayer tileMap)
    {
        if (tileMap.TileSet == null)
        {
            return;
        }

        if (tileMap.GetUsedCells().Count > 0)
        {
            return;
        }

        TileSetAtlasSource source = tileMap.TileSet.GetSource(0) as TileSetAtlasSource;
        if (source == null)
        {
            return;
        }

        Vector2I atlasTile = FindFirstTile(source);
        if (atlasTile.X < 0 || atlasTile.Y < 0)
        {
            return;
        }

        for (int x = 0; x < 30; x++)
        {
            tileMap.SetCell(new Vector2I(x, 14), 0, atlasTile, 0);
        }

        for (int x = 6; x < 12; x++)
        {
            tileMap.SetCell(new Vector2I(x, 10), 0, atlasTile, 0);
        }

        for (int x = 17; x < 24; x++)
        {
            tileMap.SetCell(new Vector2I(x, 8), 0, atlasTile, 0);
        }
    }

    private static Vector2I FindFirstTile(TileSetAtlasSource source)
    {
        Rect2I region = BuildStationTileset.TILE_ATLAS_REGION_XYWH;
        int startX = region.Position.X / BuildStationTileset.TILE_SIZE;
        int startY = region.Position.Y / BuildStationTileset.TILE_SIZE;
        int cellsX = region.Size.X / BuildStationTileset.TILE_SIZE;
        int cellsY = region.Size.Y / BuildStationTileset.TILE_SIZE;

        for (int y = 0; y < cellsY; y++)
        {
            for (int x = 0; x < cellsX; x++)
            {
                Vector2I atlas = new(startX + x, startY + y);
                Variant hasTile = source.Call("has_tile", atlas);
                if (hasTile.VariantType == Variant.Type.Bool && (bool)hasTile)
                {
                    return atlas;
                }
            }
        }

        return new Vector2I(-1, -1);
    }
}
