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
        TextureFilter = TextureFilterEnum.Nearest;
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

        string basePath = useFemale
            ? "res://Art/Characters/Female/female_"
            : "res://Art/Characters/Male/male_";

        Texture2D fallback = useFemale ? FemaleTexture : MaleTexture;

        _idleFrames = LoadFrames(basePath + "idle_", 2, fallback);
        _walkFrames = LoadFrames(basePath + "walk_", 2, fallback);
        _jumpFrames = LoadFrames(basePath + "jump_", 1, fallback);
        _fallFrames = LoadFrames(basePath + "fall_", 1, fallback);
    }

    private static Texture2D[] LoadFrames(string prefix, int frameCount, Texture2D fallback)
    {
        List<Texture2D> frames = new();

        for (int i = 0; i < frameCount; i++)
        {
            Texture2D frame = ResourceLoader.Load<Texture2D>($"{prefix}{i}.png");
            if (frame != null)
            {
                frames.Add(frame);
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
