using Godot;

[Tool]
public partial class TestLevelBuilder : RefCounted
{
    public const string SCENE_PATH = "res://Scenes/TestLevel.tscn";
    private const string TILESET_PATH = BuildStationTileset.TILESET_PATH;

    public static bool Build()
    {
        TileSet tileSet = ResourceLoader.Load<TileSet>(TILESET_PATH);
        if (tileSet == null)
        {
            GD.PushWarning("[TestLevelBuilder] TileSet missing, trying to build it first.");
            if (!BuildStationTileset.Build())
            {
                return false;
            }

            tileSet = ResourceLoader.Load<TileSet>(TILESET_PATH);
            if (tileSet == null)
            {
                GD.PushError("[TestLevelBuilder] Failed to load TileSet after build.");
                return false;
            }
        }

        TestLevelAtlasBinder root = new() { Name = "TestLevel" };

        AtlasRegistry registry = new() { Name = "AtlasRegistry" };
        root.AddChild(registry);
        registry.Owner = root;

        TileMapLayer tileMap = new()
        {
            Name = "TileMap",
            TileSet = tileSet,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled
        };

        root.AddChild(tileMap);
        tileMap.Owner = root;

        PaintSampleLayout(tileMap, tileSet);

        Sprite2D pickup = new()
        {
            Name = "OxygenPickupSprite",
            Position = new Vector2(200, 256),
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled
        };
        root.AddChild(pickup);
        pickup.Owner = root;

        Sprite2D checkpoint = new()
        {
            Name = "CheckpointSprite",
            Position = new Vector2(360, 240),
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled
        };
        root.AddChild(checkpoint);
        checkpoint.Owner = root;

        CanvasLayer ui = new() { Name = "UI" };
        root.AddChild(ui);
        ui.Owner = root;

        Control uiRoot = new() { Name = "Root" };
        uiRoot.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        ui.AddChild(uiRoot);
        uiRoot.Owner = root;

        TextureRect bar = new()
        {
            Name = "OxygenBarPreview",
            Position = new Vector2(16, 16),
            Size = new Vector2(256, 32),
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            TextureRepeat = CanvasItem.TextureRepeatEnum.Disabled
        };
        uiRoot.AddChild(bar);
        bar.Owner = root;

        PackedScene packed = new();
        Error packError = packed.Pack(root);
        if (packError != Error.Ok)
        {
            GD.PushError($"[TestLevelBuilder] Failed to pack scene: {packError}");
            return false;
        }

        Error saveError = ResourceSaver.Save(packed, SCENE_PATH);
        if (saveError != Error.Ok)
        {
            GD.PushError($"[TestLevelBuilder] Failed to save scene: {saveError}");
            return false;
        }

        GD.Print($"[TestLevelBuilder] Scene saved: {SCENE_PATH}");
        return true;
    }

    private static void PaintSampleLayout(TileMapLayer tileMap, TileSet tileSet)
    {
        int sourceId = 0;
        TileSetAtlasSource source = tileSet.GetSource(sourceId) as TileSetAtlasSource;
        if (source == null)
        {
            GD.PushWarning("[TestLevelBuilder] TileSet source 0 is not an atlas source.");
            return;
        }

        Vector2I atlasTile = FindFirstAvailableTile(source);
        if (atlasTile.X < 0 || atlasTile.Y < 0)
        {
            GD.PushWarning("[TestLevelBuilder] No available tile found in atlas.");
            return;
        }

        // Ground
        for (int x = 0; x < 30; x++)
        {
            tileMap.SetCell(new Vector2I(x, 14), sourceId, atlasTile, 0);
        }

        // Platforms
        for (int x = 6; x < 12; x++)
        {
            tileMap.SetCell(new Vector2I(x, 10), sourceId, atlasTile, 0);
        }

        for (int x = 17; x < 24; x++)
        {
            tileMap.SetCell(new Vector2I(x, 8), sourceId, atlasTile, 0);
        }
    }

    private static Vector2I FindFirstAvailableTile(TileSetAtlasSource source)
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
                Vector2I atlasCoord = new(startX + x, startY + y);
                Variant hasTile = source.Call("has_tile", atlasCoord);
                if (hasTile.VariantType == Variant.Type.Bool && (bool)hasTile)
                {
                    return atlasCoord;
                }
            }
        }

        return new Vector2I(-1, -1);
    }
}
