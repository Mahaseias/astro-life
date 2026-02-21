using Godot;
using System.Collections.Generic;

[Tool]
public partial class AtlasRegistry : Node
{
    public const string SHEET_PATH = "res://Art/Tiles/StationTiles.png";

    // Adjustable atlas regions.
    public static readonly Rect2I REGION_OXYGEN_PICKUP = new(1030, 540, 64, 64);
    public static readonly Rect2I REGION_CHECKPOINT = new(1180, 520, 96, 192);
    public static readonly Rect2I REGION_UI_OXYGEN_BAR = new(1200, 820, 320, 80);

    private readonly Dictionary<string, Rect2I> _regions = new()
    {
        { "OxygenPickup", REGION_OXYGEN_PICKUP },
        { "Checkpoint", REGION_CHECKPOINT },
        { "UI_OxygenBar", REGION_UI_OXYGEN_BAR },
    };

    private readonly Dictionary<string, AtlasTexture> _cache = new();
    private Texture2D _sheet;

    public override void _Ready()
    {
        EnsureSheetLoaded();
    }

    public AtlasTexture Get(string key)
    {
        if (_cache.TryGetValue(key, out AtlasTexture cached) && cached != null)
        {
            return cached;
        }

        if (!_regions.TryGetValue(key, out Rect2I region))
        {
            GD.PushWarning($"[AtlasRegistry] Unknown key: {key}");
            return null;
        }

        if (!EnsureSheetLoaded())
        {
            return null;
        }

        Rect2I clamped = ClampRegion(region, new Vector2I(_sheet.GetWidth(), _sheet.GetHeight()));
        if (clamped.Size.X <= 0 || clamped.Size.Y <= 0)
        {
            GD.PushWarning($"[AtlasRegistry] Invalid region for key {key}: {region}");
            return null;
        }

        AtlasTexture atlas = new()
        {
            Atlas = _sheet,
            Region = clamped
        };

        _cache[key] = atlas;
        return atlas;
    }

    private bool EnsureSheetLoaded()
    {
        if (_sheet != null)
        {
            return true;
        }

        _sheet = ResourceLoader.Load<Texture2D>(SHEET_PATH);
        if (_sheet == null)
        {
            GD.PushWarning($"[AtlasRegistry] Missing spritesheet: {SHEET_PATH}");
            return false;
        }

        return true;
    }

    private static Rect2I ClampRegion(Rect2I region, Vector2I maxSize)
    {
        int x = Mathf.Clamp(region.Position.X, 0, maxSize.X);
        int y = Mathf.Clamp(region.Position.Y, 0, maxSize.Y);
        int w = Mathf.Clamp(region.Size.X, 0, maxSize.X - x);
        int h = Mathf.Clamp(region.Size.Y, 0, maxSize.Y - y);
        return new Rect2I(x, y, w, h);
    }
}
