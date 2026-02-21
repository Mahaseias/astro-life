using Godot;
using System.Collections.Generic;

public partial class RespawnManager : Node
{
    public static RespawnManager Instance { get; private set; }

    [Signal] public delegate void CheckpointChangedEventHandler(string sceneKey, string checkpointLabel);

    private readonly Dictionary<string, Vector2> _defaultSpawnByScene = new();
    private readonly Dictionary<string, Vector2> _checkpointByScene = new();
    private readonly Dictionary<string, string> _checkpointLabelByScene = new();

    private PlayerController _player;

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

    public void RegisterPlayer(PlayerController player)
    {
        _player = player;

        string sceneKey = GetSceneKey();
        if (!_defaultSpawnByScene.ContainsKey(sceneKey))
        {
            _defaultSpawnByScene[sceneKey] = player.GlobalPosition;
        }

        if (!_checkpointByScene.ContainsKey(sceneKey))
        {
            _checkpointByScene[sceneKey] = _defaultSpawnByScene[sceneKey];
            _checkpointLabelByScene[sceneKey] = "spawn";
        }

        EmitSignal(SignalName.CheckpointChanged, sceneKey, _checkpointLabelByScene[sceneKey]);
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (_player == player)
        {
            _player = null;
        }
    }

    public void RegisterDefaultSpawn(Vector2 spawnPosition)
    {
        _defaultSpawnByScene[GetSceneKey()] = spawnPosition;
    }

    public void SetCheckpoint(Vector2 position, string checkpointLabel)
    {
        string sceneKey = GetSceneKey();
        _checkpointByScene[sceneKey] = position;
        _checkpointLabelByScene[sceneKey] = string.IsNullOrWhiteSpace(checkpointLabel) ? "checkpoint" : checkpointLabel;
        EmitSignal(SignalName.CheckpointChanged, sceneKey, _checkpointLabelByScene[sceneKey]);
    }

    public string GetActiveCheckpointLabel()
    {
        string sceneKey = GetSceneKey();
        if (_checkpointLabelByScene.TryGetValue(sceneKey, out string label))
        {
            return label;
        }

        return "spawn";
    }

    public PlayerController GetCurrentPlayer()
    {
        return _player;
    }

    public void RequestRespawn()
    {
        ForceRespawn();
    }

    public void ForceRespawn()
    {
        if (_player == null)
        {
            return;
        }

        Vector2 respawnPosition = GetRespawnPositionForCurrentScene();
        _player.RespawnAt(respawnPosition);
    }

    private Vector2 GetRespawnPositionForCurrentScene()
    {
        string sceneKey = GetSceneKey();
        if (_checkpointByScene.TryGetValue(sceneKey, out Vector2 checkpoint))
        {
            return checkpoint;
        }

        if (_defaultSpawnByScene.TryGetValue(sceneKey, out Vector2 defaultSpawn))
        {
            return defaultSpawn;
        }

        return _player != null ? _player.GlobalPosition : Vector2.Zero;
    }

    private string GetSceneKey()
    {
        Node scene = GetTree().CurrentScene;
        if (scene == null)
        {
            return "unknown";
        }

        if (!string.IsNullOrEmpty(scene.SceneFilePath))
        {
            return scene.SceneFilePath;
        }

        return scene.Name;
    }
}
