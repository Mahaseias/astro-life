using Godot;
using System.Collections.Generic;

public partial class PlayerVisualController : Sprite2D
{
    [Export] public Texture2D MaleTexture;
    [Export] public Texture2D FemaleTexture;

    private PlayerController _player;
    private Texture2D[] _idleFrames;
    private Texture2D[] _walkFrames;
    private Texture2D[] _jumpFrames;
    private Texture2D[] _fallFrames;

    private string _currentState = "idle";
    private float _frameTimer;
    private int _frameIndex;

    public override void _Ready()
    {
        TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        _player = GetParentOrNull<PlayerController>();
        LoadCharacterFrames();
        SetState("idle");
        ApplyFrame();
    }

    public override void _Process(double delta)
    {
        if (_player == null)
        {
            return;
        }

        float horizontalSpeed = Mathf.Abs(_player.Velocity.X);
        if (!_player.IsOnFloor())
        {
            SetState(_player.Velocity.Y < 0f ? "jump" : "fall");
        }
        else if (horizontalSpeed > 8f)
        {
            SetState("walk");
        }
        else
        {
            SetState("idle");
        }

        UpdateFrame((float)delta);
        UpdateFacing();
    }

    private void LoadCharacterFrames()
    {
        bool useFemale = GameSession.Instance != null && GameSession.Instance.SelectedCharacter == GameSession.CharacterFemale;

        Texture2D fallback = useFemale ? FemaleTexture : MaleTexture;
        string key = useFemale ? AssetRegistry.PlayerFemale : AssetRegistry.PlayerMale;
        Texture2D spritesheet = AssetRegistry.Instance != null
            ? AssetRegistry.Instance.GetTexture(key, null)
            : null;

        _idleFrames = LoadFramesFromSheet(spritesheet, fallback, 0, 1);
        _walkFrames = LoadFramesFromSheet(spritesheet, fallback, 2, 3);
        _jumpFrames = LoadFramesFromSheet(spritesheet, fallback, 4);
        _fallFrames = LoadFramesFromSheet(spritesheet, fallback, 5);
    }

    private static Texture2D[] LoadFramesFromSheet(Texture2D spritesheet, Texture2D fallback, params int[] frameIndices)
    {
        List<Texture2D> frames = new();

        if (spritesheet != null)
        {
            Vector2 sheetSize = spritesheet.GetSize();
            int maxFrame = frameIndices.Length > 0 ? frameIndices[frameIndices.Length - 1] : 0;
            bool validSheet = sheetSize.X >= (maxFrame + 1) * 64 && sheetSize.Y >= 64;

            if (validSheet)
            {
                foreach (int frameIndex in frameIndices)
                {
                    AtlasTexture atlasFrame = new()
                    {
                        Atlas = spritesheet,
                        Region = new Rect2(frameIndex * 64, 0, 64, 64)
                    };
                    frames.Add(atlasFrame);
                }
            }
        }

        if (frames.Count == 0)
        {
            foreach (int _ in frameIndices)
            {
                if (fallback != null)
                {
                    frames.Add(fallback);
                }
            }
        }

        if (frames.Count == 0 && fallback != null)
        {
            frames.Add(fallback);
        }

        return frames.ToArray();
    }

    private void SetState(string state)
    {
        if (_currentState == state)
        {
            return;
        }

        _currentState = state;
        _frameTimer = 0f;
        _frameIndex = 0;
        ApplyFrame();
    }

    private void UpdateFrame(float delta)
    {
        Texture2D[] frames = GetFramesForCurrentState();
        if (frames.Length <= 1)
        {
            return;
        }

        _frameTimer += delta;
        float frameDuration = _currentState == "walk" ? 0.11f : 0.35f;
        if (_frameTimer < frameDuration)
        {
            return;
        }

        _frameTimer = 0f;
        _frameIndex = (_frameIndex + 1) % frames.Length;
        ApplyFrame();
    }

    private Texture2D[] GetFramesForCurrentState()
    {
        return _currentState switch
        {
            "walk" => _walkFrames,
            "jump" => _jumpFrames,
            "fall" => _fallFrames,
            _ => _idleFrames,
        };
    }

    private void ApplyFrame()
    {
        Texture2D[] frames = GetFramesForCurrentState();
        if (frames.Length == 0)
        {
            return;
        }

        int clampedIndex = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
        Texture = frames[clampedIndex];
    }

    private void UpdateFacing()
    {
        if (Mathf.Abs(_player.Velocity.X) > 1f)
        {
            FlipH = _player.Velocity.X < 0f;
        }
    }
}
