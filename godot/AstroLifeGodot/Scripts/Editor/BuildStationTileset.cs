using Godot;

[Tool]
public partial class BuildStationTileset : EditorScript
{
    public const string SHEET_PATH = "res://Art/Tiles/StationTiles.png";
    public const string TILESET_PATH = "res://Tilesets/StationTileset.tres";
    public const int TILE_SIZE = 32;

    // Tiles region (top-left quadrant) inside StationTiles.png (1536x1024).
    public static readonly Rect2I TILE_ATLAS_REGION_XYWH = new(0, 0, 1024, 512);

    public override void _Run()
    {
        Build();
    }

    public static bool Build()
    {
        Texture2D sheet = ResourceLoader.Load<Texture2D>(SHEET_PATH);
        if (sheet == null)
        {
            GD.PushError($"[BuildStationTileset] Missing spritesheet: {SHEET_PATH}");
            return false;
        }

        Image image = sheet.GetImage();
        if (image == null)
        {
            GD.PushError("[BuildStationTileset] Could not read spritesheet pixels.");
            return false;
        }

        Vector2I sheetSize = new(sheet.GetWidth(), sheet.GetHeight());
        Rect2I region = ClampRegion(TILE_ATLAS_REGION_XYWH, sheetSize);

        TileSet tileSet = new()
        {
            TileSize = new Vector2I(TILE_SIZE, TILE_SIZE)
        };

        TileSetAtlasSource atlas = new()
        {
            Texture = sheet,
            TextureRegionSize = new Vector2I(TILE_SIZE, TILE_SIZE)
        };

        atlas.Set("margins", Vector2I.Zero);
        atlas.Set("separation", Vector2I.Zero);

        int sourceId = tileSet.AddSource(atlas);
        int created = CreateTiles(atlas, image, region);

        Error saveError = ResourceSaver.Save(tileSet, TILESET_PATH);
        if (saveError != Error.Ok)
        {
            GD.PushError($"[BuildStationTileset] Failed to save {TILESET_PATH}: {saveError}");
            return false;
        }

        GD.Print($"[BuildStationTileset] Source={sourceId}, tiles={created}, path={TILESET_PATH}");
        return true;
    }

    private static int CreateTiles(TileSetAtlasSource atlas, Image image, Rect2I regionPx)
    {
        int created = 0;
        int startCellX = regionPx.Position.X / TILE_SIZE;
        int startCellY = regionPx.Position.Y / TILE_SIZE;
        int cellsX = regionPx.Size.X / TILE_SIZE;
        int cellsY = regionPx.Size.Y / TILE_SIZE;

        for (int y = 0; y < cellsY; y++)
        {
            for (int x = 0; x < cellsX; x++)
            {
                int pixelX = regionPx.Position.X + (x * TILE_SIZE);
                int pixelY = regionPx.Position.Y + (y * TILE_SIZE);

                if (IsFullyTransparent(image, pixelX, pixelY))
                {
                    continue;
                }

                Vector2I atlasCoords = new(startCellX + x, startCellY + y);
                atlas.CreateTile(atlasCoords);
                created++;
            }
        }

        return created;
    }

    private static bool IsFullyTransparent(Image image, int startX, int startY)
    {
        for (int py = 0; py < TILE_SIZE; py++)
        {
            for (int px = 0; px < TILE_SIZE; px++)
            {
                Color color = image.GetPixel(startX + px, startY + py);
                if (color.A > 0.01f)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static Rect2I ClampRegion(Rect2I region, Vector2I maxSize)
    {
        int x = Mathf.Clamp(region.Position.X, 0, maxSize.X);
        int y = Mathf.Clamp(region.Position.Y, 0, maxSize.Y);
        int w = Mathf.Clamp(region.Size.X, 0, maxSize.X - x);
        int h = Mathf.Clamp(region.Size.Y, 0, maxSize.Y - y);

        w -= w % TILE_SIZE;
        h -= h % TILE_SIZE;

        return new Rect2I(x, y, w, h);
    }
}
