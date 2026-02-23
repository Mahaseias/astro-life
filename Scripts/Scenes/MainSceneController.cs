using Godot;

public partial class MainSceneController : Node2D
{
    private static readonly Vector2 DefaultSpawn = new(180f, 260f);

    public override void _Ready()
    {
        SetupLevel();
    }

    public void SetupLevel()
    {
        AddChild(CreateBackgroundCanvas());

        TileMap tileMap = CreateTileMap();
        AddChild(tileMap);

        StaticBody2D floor = CreateFloorBody();
        AddChild(floor);

        Node2D groundVisual = CreateGroundVisualStrip();
        AddChild(groundVisual);

        OxygenSystem oxygenSystem = new()
        {
            Name = "OxygenSystem",
            MaxOxygen = 100f,
            StartingOxygen = 100f,
            BaseRate = 5f,
            Multiplier = 1f,
        };
        AddChild(oxygenSystem);

        PlayerController player = CreatePlayer();
        AddChild(player);
        player.BindOxygenSystem(oxygenSystem);

        BossController boss = CreateBoss(player);
        AddChild(boss);

        HudController hud = CreateHud(oxygenSystem);
        AddChild(hud);

        Vector2 spawn = GameManager.Instance != null && GameManager.Instance.HasCheckpoint
            ? GameManager.Instance.CurrentCheckpoint
            : DefaultSpawn;

        player.GlobalPosition = spawn;

        if (GameManager.Instance != null && !GameManager.Instance.HasCheckpoint)
        {
            GameManager.Instance.SetCheckpoint(spawn);
        }
    }

    private CanvasLayer CreateBackgroundCanvas()
    {
        CanvasLayer layer = new()
        {
            Name = "BackgroundLayer",
            Layer = -20,
        };

        TextureRect background = new()
        {
            Name = "BackgroundTexture",
            Texture = AssetManager.Instance?.GetTexture(AssetManager.KeyBackground),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
            Modulate = new Color(1f, 1f, 1f, 0.55f),
        };
        background.SetAnchorsPreset(Control.LayoutPreset.FullRect);

        NinePatchRect stationWindow = new()
        {
            Name = "StationWindow",
            Position = new Vector2(10f, 10f),
            Size = new Vector2(300f, 90f),
            Texture = AssetManager.Instance?.GetStationWindowTexture(),
            Modulate = new Color(1f, 1f, 1f, 0.35f),
        };
        stationWindow.PatchMarginLeft = 8;
        stationWindow.PatchMarginTop = 8;
        stationWindow.PatchMarginRight = 8;
        stationWindow.PatchMarginBottom = 8;

        layer.AddChild(background);
        layer.AddChild(stationWindow);
        return layer;
    }

    private TileMap CreateTileMap()
    {
        TileMap tileMap = new()
        {
            Name = "TileMap",
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            TileSet = CreateRuntimeTileSet(),
            ZIndex = -1,
        };

        if (tileMap.GetLayersCount() == 0)
        {
            tileMap.AddLayer(0);
        }

        for (int x = -8; x <= 36; x++)
        {
            tileMap.SetCell(0, new Vector2I(x, 12), 0, AssetManager.StationGroundAtlasCoord);
        }

        for (int x = 6; x <= 10; x++)
        {
            tileMap.SetCell(0, new Vector2I(x, 9), 0, AssetManager.StationGroundAtlasCoord);
        }

        return tileMap;
    }

    private TileSet CreateRuntimeTileSet()
    {
        Texture2D tileTexture = AssetManager.Instance?.GetTexture(AssetManager.KeyStationTiles);

        TileSet tileSet = new()
        {
            TileSize = new Vector2I(32, 32),
        };

        if (tileTexture == null)
        {
            return tileSet;
        }

        TileSetAtlasSource source = new()
        {
            Texture = tileTexture,
            TextureRegionSize = new Vector2I(32, 32),
        };
        source.CreateTile(AssetManager.StationGroundAtlasCoord);

        tileSet.AddSource(source, 0);
        return tileSet;
    }

    private static StaticBody2D CreateFloorBody()
    {
        StaticBody2D floor = new()
        {
            Name = "Floor",
            Position = new Vector2(480f, 430f),
        };

        CollisionShape2D collisionShape = new()
        {
            Name = "CollisionShape2D",
            Shape = new RectangleShape2D
            {
                Size = new Vector2(2200f, 80f),
            },
        };

        floor.AddChild(collisionShape);
        return floor;
    }

    private Node2D CreateGroundVisualStrip()
    {
        Node2D root = new()
        {
            Name = "GroundVisual",
            ZIndex = 1,
        };

        Texture2D tileTexture = AssetManager.Instance?.GetStationGroundTile();
        if (tileTexture == null)
        {
            return root;
        }

        for (int x = -8; x <= 40; x++)
        {
            Sprite2D top = new()
            {
                Name = $"GroundTop_{x}",
                Texture = tileTexture,
                Centered = false,
                Position = new Vector2(x * 32f, 384f),
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            };

            Sprite2D bottom = new()
            {
                Name = $"GroundBottom_{x}",
                Texture = tileTexture,
                Centered = false,
                Position = new Vector2(x * 32f, 416f),
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                Modulate = new Color(0.8f, 0.8f, 0.8f, 1f),
            };

            root.AddChild(top);
            root.AddChild(bottom);
        }

        return root;
    }

    private PlayerController CreatePlayer()
    {
        PlayerController player = new()
        {
            Name = "Player",
            Position = DefaultSpawn,
            MoveSpeed = 240f,
            JumpVelocity = -390f,
        };

        CollisionShape2D collisionShape = new()
        {
            Name = "CollisionShape2D",
            Position = new Vector2(0f, 14f),
            Shape = new RectangleShape2D
            {
                Size = new Vector2(22f, 34f),
            },
        };

        AnimatedSprite2D animatedSprite = new()
        {
            Name = "AnimatedSprite2D",
            Position = new Vector2(0f, -2f),
            Scale = new Vector2(2.0f, 2.0f),
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            SpriteFrames = AssetManager.Instance?.BuildPlayerFrames(
                GameManager.Instance?.SelectedCharacter ?? GameManager.CharacterOption.AstroMasc),
            ZIndex = 2,
        };
        animatedSprite.Play("idle");

        Camera2D camera = new()
        {
            Name = "Camera2D",
            Enabled = true,
            PositionSmoothingEnabled = true,
            PositionSmoothingSpeed = 4f,
            LimitLeft = -500,
            LimitRight = 1700,
            LimitTop = -200,
            LimitBottom = 700,
        };

        player.AddChild(collisionShape);
        player.AddChild(animatedSprite);
        player.AddChild(camera);
        return player;
    }

    private BossController CreateBoss(PlayerController player)
    {
        BossController boss = new()
        {
            Name = "Boss",
            Position = new Vector2(860f, 285f),
        };

        Sprite2D sprite = new()
        {
            Name = "BossSprite",
            Texture = AssetManager.Instance?.GetBossTexture(),
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            RegionEnabled = true,
            RegionRect = AssetManager.BossCollectorVisibleRegion,
            ZIndex = 2,
        };

        Area2D coreArea = new()
        {
            Name = "CoreArea",
            Position = new Vector2(0f, 0f),
        };

        CollisionShape2D coreCollision = new()
        {
            Name = "CollisionShape2D",
            Shape = new RectangleShape2D
            {
                Size = new Vector2(72f, 72f),
            },
        };

        coreArea.AddChild(coreCollision);
        boss.AddChild(sprite);
        boss.AddChild(coreArea);
        boss.AssignPlayer(player);

        return boss;
    }

    private HudController CreateHud(OxygenSystem oxygenSystem)
    {
        HudController hud = new()
        {
            Name = "UI",
            Layer = 10,
        };

        Control root = new()
        {
            Name = "Root",
            AnchorRight = 1f,
            AnchorBottom = 1f,
            OffsetLeft = 0f,
            OffsetTop = 0f,
            OffsetRight = 0f,
            OffsetBottom = 0f,
        };

        TextureProgressBar oxygenBar = new()
        {
            Name = "OxygenBar",
            Position = new Vector2(24f, 24f),
            Size = new Vector2(260f, 24f),
            MinValue = 0,
            MaxValue = 100,
            Value = 100,
            TextureUnder = AssetManager.Instance?.GetUiO2UnderTexture(),
            TextureProgress = AssetManager.Instance?.GetUiO2ProgressTexture(),
            StretchMarginLeft = 0,
            StretchMarginTop = 0,
            StretchMarginRight = 0,
            StretchMarginBottom = 0,
        };

        Label oxygenLabel = new()
        {
            Name = "OxygenLabel",
            Position = new Vector2(24f, 54f),
            Text = "O2: 100/100",
        };
        oxygenLabel.AddThemeFontSizeOverride("font_size", 18);

        root.AddChild(oxygenBar);
        root.AddChild(oxygenLabel);

        hud.AddChild(root);
        hud.OxygenBarPath = new NodePath("Root/OxygenBar");
        hud.OxygenLabelPath = new NodePath("Root/OxygenLabel");
        hud.BindOxygenSystem(oxygenSystem);
        return hud;
    }
}
