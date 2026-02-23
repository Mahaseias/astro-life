using Godot;
using System.Collections.Generic;

public partial class AssetManager : Node
{
    public const string KeyAstroMasc = "astro_masc";
    public const string KeyAstroFem = "astro_fem";
    public const string KeyBossCollector = "boss_collector";
    public const string KeyUiOxygenBar = "ui_oxygen_bar";
    public const string KeyStationTiles = "station_tiles";
    public const string KeyBackground = "background";

    // Player slicing requested in the latest spec.
    public static readonly Rect2 PlayerIdleRegion = new(0, 0, 32, 32);
    public static readonly Rect2 PlayerRunFrame1Region = new(32, 0, 32, 32);
    public static readonly Rect2 PlayerJumpRegion = new(0, 32, 32, 32);

    // Station tiles (32x32).
    public static readonly Rect2 StationGroundRegion = new(128, 64, 32, 32);
    public static readonly Rect2 StationWindowRegion = new(224, 32, 32, 32);
    public static readonly Vector2I StationGroundAtlasCoord = new(4, 2);

    // Crop boss to hide colored generation borders.
    public static readonly Rect2 BossCollectorVisibleRegion = new(73, 70, 128, 128);

    public static readonly Rect2 UiO2UnderRegion = new(13, 6, 64, 16);
    public static readonly Rect2 UiO2ProgressRegion = new(13, 24, 64, 16);

    public static AssetManager Instance { get; private set; }

    private readonly Dictionary<string, string> _paths = new()
    {
        { KeyAstroMasc, "res://Images/Astro_Masc_Sheet.png" },
        { KeyAstroFem, "res://Images/Astro_Fem_Sheet.png" },
        { KeyBossCollector, "res://Images/Boss_Collector.png.png" },
        { KeyUiOxygenBar, "res://Images/UI_Oxygen_Bar.png" },
        { KeyStationTiles, "res://Images/Tileset_Station.png" },
        { KeyBackground, "res://Images/1.jpg" },
    };

    private readonly HashSet<string> _transparentKeys = new()
    {
        KeyAstroMasc,
        KeyAstroFem,
        KeyBossCollector,
    };

    private readonly Dictionary<string, Texture2D> _textures = new();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public override void _Ready()
    {
        foreach ((string key, string _) in _paths)
        {
            EnsureLoaded(key);
        }
    }

    public Texture2D GetTexture(string key)
    {
        EnsureLoaded(key);
        _textures.TryGetValue(key, out Texture2D texture);
        return texture;
    }

    public AtlasTexture CreateAtlasTexture(string key, Rect2 requestedRegion)
    {
        Texture2D atlas = GetTexture(key);
        if (atlas == null)
        {
            return null;
        }

        return new AtlasTexture
        {
            Atlas = atlas,
            Region = FitRegionToAtlas(atlas, requestedRegion),
        };
    }

    public SpriteFrames BuildPlayerFrames(GameManager.CharacterOption character)
    {
        string key = character == GameManager.CharacterOption.AstroFem ? KeyAstroFem : KeyAstroMasc;

        SpriteFrames frames = new();

        frames.AddAnimation("idle");
        frames.SetAnimationSpeed("idle", 6f);
        frames.SetAnimationLoop("idle", true);
        AtlasTexture idle = CreateAtlasTexture(key, PlayerIdleRegion);
        if (idle != null)
        {
            frames.AddFrame("idle", idle);
        }

        frames.AddAnimation("run");
        frames.SetAnimationSpeed("run", 10f);
        frames.SetAnimationLoop("run", true);
        AtlasTexture run = CreateAtlasTexture(key, PlayerRunFrame1Region);
        if (run != null)
        {
            frames.AddFrame("run", run);
            frames.AddFrame("run", run);
        }

        frames.AddAnimation("jump");
        frames.SetAnimationSpeed("jump", 1f);
        frames.SetAnimationLoop("jump", false);
        AtlasTexture jump = CreateAtlasTexture(key, PlayerJumpRegion);
        if (jump != null)
        {
            frames.AddFrame("jump", jump);
        }

        return frames;
    }

    public Texture2D GetBossTexture()
    {
        return GetTexture(KeyBossCollector);
    }

    public Texture2D GetStationGroundTile()
    {
        return CreateAtlasTexture(KeyStationTiles, StationGroundRegion);
    }

    public Texture2D GetStationWindowTexture()
    {
        return CreateAtlasTexture(KeyStationTiles, StationWindowRegion);
    }

    public Texture2D GetUiO2ProgressTexture()
    {
        return CreateAtlasTexture(KeyUiOxygenBar, UiO2ProgressRegion);
    }

    public Texture2D GetUiO2UnderTexture()
    {
        return CreateAtlasTexture(KeyUiOxygenBar, UiO2UnderRegion);
    }

    private void EnsureLoaded(string key)
    {
        if (_textures.ContainsKey(key))
        {
            return;
        }

        if (!_paths.TryGetValue(key, out string path))
        {
            GD.PushWarning($"Asset key not mapped: {key}");
            return;
        }

        if (!FileAccess.FileExists(path))
        {
            GD.PushWarning($"Asset not found: {path}");
            return;
        }

        Texture2D texture = ResourceLoader.Load<Texture2D>(path);
        if (texture == null)
        {
            GD.PushWarning($"Could not load texture at: {path}");
            return;
        }

        _textures[key] = _transparentKeys.Contains(key) ? BuildTransparentTexture(texture) : texture;
    }

    private static Texture2D BuildTransparentTexture(Texture2D source)
    {
        Image image = source.GetImage();
        if (image == null)
        {
            return source;
        }

        image.Convert(Image.Format.Rgba8);

        int width = image.GetWidth();
        int height = image.GetHeight();
        if (width <= 0 || height <= 0)
        {
            return source;
        }

        Color baseColor = image.GetPixel(0, 0);
        bool[] visited = new bool[width * height];
        Queue<Vector2I> queue = new();

        void EnqueueIfCandidate(int x, int y)
        {
            int idx = (y * width) + x;
            if (visited[idx])
            {
                return;
            }

            Color c = image.GetPixel(x, y);
            bool nearWhite = c.R > 0.94f && c.G > 0.94f && c.B > 0.94f;
            bool nearBase = Mathf.Abs(c.R - baseColor.R) < 0.07f && Mathf.Abs(c.G - baseColor.G) < 0.07f && Mathf.Abs(c.B - baseColor.B) < 0.07f;
            if (!nearWhite && !nearBase)
            {
                return;
            }

            visited[idx] = true;
            queue.Enqueue(new Vector2I(x, y));
        }

        for (int x = 0; x < width; x++)
        {
            EnqueueIfCandidate(x, 0);
            EnqueueIfCandidate(x, height - 1);
        }

        for (int y = 0; y < height; y++)
        {
            EnqueueIfCandidate(0, y);
            EnqueueIfCandidate(width - 1, y);
        }

        while (queue.Count > 0)
        {
            Vector2I p = queue.Dequeue();
            Color c = image.GetPixel(p.X, p.Y);
            image.SetPixel(p.X, p.Y, new Color(c.R, c.G, c.B, 0f));

            if (p.X > 0)
            {
                EnqueueIfCandidate(p.X - 1, p.Y);
            }

            if (p.X < width - 1)
            {
                EnqueueIfCandidate(p.X + 1, p.Y);
            }

            if (p.Y > 0)
            {
                EnqueueIfCandidate(p.X, p.Y - 1);
            }

            if (p.Y < height - 1)
            {
                EnqueueIfCandidate(p.X, p.Y + 1);
            }
        }

        return ImageTexture.CreateFromImage(image);
    }

    private static Rect2 FitRegionToAtlas(Texture2D atlas, Rect2 region)
    {
        float atlasWidth = Mathf.Max(1f, atlas.GetWidth());
        float atlasHeight = Mathf.Max(1f, atlas.GetHeight());

        float x = Mathf.Clamp(region.Position.X, 0f, atlasWidth - 1f);
        float y = Mathf.Clamp(region.Position.Y, 0f, atlasHeight - 1f);

        float width = Mathf.Clamp(region.Size.X, 1f, atlasWidth - x);
        float height = Mathf.Clamp(region.Size.Y, 1f, atlasHeight - y);

        return new Rect2(x, y, width, height);
    }
}
