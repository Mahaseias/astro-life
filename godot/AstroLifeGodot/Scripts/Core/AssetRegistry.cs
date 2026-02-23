using Godot;
using System.Collections.Generic;

public partial class AssetRegistry : Node
{
    public const string PlayerMale = "PlayerMale";
    public const string PlayerFemale = "PlayerFemale";
    public const string TileMetalA = "Tile_Metal_A";
    public const string PlatformMetalA = "Platform_Metal_A";
    public const string OxygenPickup = "OxygenPickup";
    public const string OxygenPickupGlow = "OxygenPickup_Glow";
    public const string Checkpoint = "Checkpoint";
    public const string CheckpointHolo = "Checkpoint_Holo";
    public const string UiOxygenBar = "UI_OxygenBar";
    public const string UiOxygenBarFill = "UI_OxygenBarFill";
    public const string UiOxygenIcon = "UI_OxygenIcon";
    public const string UiPanel = "UI_Panel";
    public const string BossBody = "Boss_Body";
    public const string BossCore = "Boss_Core";
    public const string Portal = "Portal";
    public const string HazardKillZone = "Hazard_KillZone";
    public const string HazardLaser = "Hazard_Laser";
    public const string HazardVapor = "Hazard_Vapor";
    public const string BackgroundSpace = "Background_Space";
    public const string BackgroundW11 = "Background_W1_1";
    public const string BackgroundW12 = "Background_W1_2";
    public const string BackgroundBoss = "Background_Boss";
    public const string PropSatellite = "Prop_Satellite";
    public const string PropSolarPanel = "Prop_SolarPanel";
    public const string PropStationModule = "Prop_StationModule";
    public const string PortraitMale = "Portrait_Male";
    public const string PortraitFemale = "Portrait_Female";

    public static AssetRegistry Instance { get; private set; }

    private readonly Dictionary<string, string> _pathByKey = new()
    {
        { PlayerMale, "res://Art/Characters/PlayerMale.png" },
        { PlayerFemale, "res://Art/Characters/PlayerFemale.png" },
        { TileMetalA, "res://Art/Tiles/StationTiles.png" },
        { PlatformMetalA, "res://Art/Tiles/Platform_Metal_A.png" },
        { OxygenPickup, "res://Art/Props/OxygenPickup.png" },
        { OxygenPickupGlow, "res://Art/Props/OxygenPickup_Glow.png" },
        { Checkpoint, "res://Art/Props/Checkpoint.png" },
        { CheckpointHolo, "res://Art/Props/Checkpoint_Holo.png" },
        { UiOxygenBar, "res://Art/UI/OxygenBar.png" },
        { UiOxygenBarFill, "res://Art/UI/OxygenBarFill.png" },
        { UiOxygenIcon, "res://Art/UI/OxygenIcon.png" },
        { UiPanel, "res://Art/UI/Panel16.png" },
        { BossBody, "res://Art/Enemies/CollectorBoss.png" },
        { BossCore, "res://Art/Enemies/CollectorBossCore.png" },
        { Portal, "res://Art/Systems/portal_gate_48x64.png" },
        { HazardKillZone, "res://Art/Props/KillZoneField.png" },
        { HazardLaser, "res://Art/Hazards/LaserBlue.png" },
        { HazardVapor, "res://Art/Hazards/OxygenVapor.png" },
        { BackgroundSpace, "res://Art/Backgrounds/SpaceBg.png" },
        { BackgroundW11, "res://Art/Backgrounds/space_bg_w1_1.png" },
        { BackgroundW12, "res://Art/Backgrounds/space_bg_w1_2.png" },
        { BackgroundBoss, "res://Art/Backgrounds/space_bg_boss.png" },
        { PropSatellite, "res://Art/Props/Satellite.png" },
        { PropSolarPanel, "res://Art/Props/SolarPanel.png" },
        { PropStationModule, "res://Art/Props/StationModule.png" },
        { PortraitMale, "res://Art/Characters/astronaut_male_portrait.png" },
        { PortraitFemale, "res://Art/Characters/astronaut_female_portrait.png" },
    };

    private readonly Dictionary<string, Texture2D> _cache = new();
    private readonly HashSet<string> _warned = new();

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

    public Texture2D GetTexture(string key, Texture2D fallback = null)
    {
        if (!_pathByKey.TryGetValue(key, out string path))
        {
            WarnOnce($"Missing art asset: {key} (unmapped key)");
            return fallback;
        }

        if (_cache.TryGetValue(path, out Texture2D cached) && cached != null)
        {
            return cached;
        }

        if (!FileAccess.FileExists(path))
        {
            WarnOnce($"Missing art asset: {key} -> {path}");
            return fallback;
        }

        Texture2D texture = ResourceLoader.Load<Texture2D>(path);
        if (texture == null)
        {
            WarnOnce($"Missing art asset: {key} -> {path}");
            return fallback;
        }

        _cache[path] = texture;
        return texture;
    }

    public void ApplyNearest(CanvasItem item)
    {
        if (item != null)
        {
            item.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        }
    }

    private void WarnOnce(string message)
    {
        if (_warned.Add(message))
        {
            GD.PushWarning(message);
        }
    }
}
