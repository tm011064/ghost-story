using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Assets.Scripts.GhostStory.Behaviours;
using UnityEngine;

public class GhostStoryGameContext : MonoBehaviour, IDontDestroyOnLoad
{
  public static GhostStoryGameContext Instance;

  [HideInInspector]
  public GhostStoryGameState GameState;

  [HideInInspector]
  public GhostStoryDefaultGameSettings GameSettings;

  private ILookup<LayerUniverseKey, GameObject> _gameObjectsByLayerUniverseKey;

  private ILookup<LayerUniverseKey, IFreezable> _freezeableGameObjectsByLayerUniverseKey;

  private FreezableTimer _freezableTimer;

  public event Action<GhostStoryGameState> GameStateChanged;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else if (Instance != this)
    {
      Destroy(gameObject);
    }

    Reset();
  }

  void Start()
  {
    UpdateGameState(
      LoadGameState());

    DisableAndHideAllObjects();
    SwitchLayer(LevelLayer.House);
    SwitchToRealWorld();
  }

  public void OnSceneLoad()
  {
    Reset();

    DisableAndHideAllObjects();
    SwitchLayer(LevelLayer.House);
    SwitchToRealWorld();
  }

  public void Reset()
  {
    GameSettings = gameObject.GetComponentOrThrow<GhostStoryDefaultGameSettings>();

    _freezableTimer = gameObject.GetComponentOrThrow<FreezableTimer>();

    var items = GameObject
      .FindObjectsOfType<LevelObjectConfig>()
      .Select(config => new
      {
        Keys = CreateKeys(config).ToArray(),
        GameObject = config.gameObject,
        FreezableComponent = config.gameObject.GetComponent<IFreezable>()
      })
      .SelectMany(c => c.Keys.Select(k => new { Key = k, GameObject = c.GameObject, FreezableComponent = c.FreezableComponent }))
      .ToArray();

    _gameObjectsByLayerUniverseKey = items
      .Where(i => i.FreezableComponent == null)
      .ToLookup(c => c.Key, c => c.GameObject);

    _freezeableGameObjectsByLayerUniverseKey = items
      .Where(i => i.FreezableComponent != null)
      .ToLookup(c => c.Key, c => c.FreezableComponent);
  }

  private void UpdateGameState(GhostStoryGameState gameState)
  {
    GameState = gameState;

    NotifyGameStateChanged();
  }

  private IEnumerable<LayerUniverseKey> CreateKeys(LevelObjectConfig levelObjectConfig)
  {
    if (levelObjectConfig.Universe == Universe.Global)
    {
      yield return new LayerUniverseKey { Layer = levelObjectConfig.Layer, Universe = Universe.AlternateWorld };
      yield return new LayerUniverseKey { Layer = levelObjectConfig.Layer, Universe = Universe.RealWorld };

      yield break;
    }

    yield return new LayerUniverseKey { Layer = levelObjectConfig.Layer, Universe = levelObjectConfig.Universe };
  }

  public void OnInventoryItemAcquired(InventoryItem inventoryItem)
  {
    inventoryItem.IsAvailable = true;

    NotifyGameStateChanged();
  }

  public void NotifyGameStateChanged()
  {
    var handler = GameStateChanged;
    if (handler != null)
    {
      handler.Invoke(GameState);
    }
  }

  private void CreateDefaultGameState(string fileName)
  {
    var weapons = GameManager.Instance
      .GetPlayerControllers()
      .SelectMany(p => p.Weapons.Select(w => new InventoryItem
      {
        Name = w.Name,
        IsAvailable = true,
        IsActive = false
      }))
      .ToArray();

    var doorKeys = Enum.GetValues(typeof(DoorKey))
      .Cast<DoorKey>()
      .Select(doorKey => new InventoryItem { Name = doorKey.ToString() })
      .ToArray();

    var misaHealth = GameManager.Instance.GetPlayerByName(PlayableCharacterNames.Misa.ToString()).Health.HealthUnits;
    var kinoHealth = GameManager.Instance.GetPlayerByName(PlayableCharacterNames.Misa.ToString()).Health.HealthUnits;

    var gameState = new GhostStoryGameState
    {
      ActiveUniverse = new LayerUniverseKey { Layer = LevelLayer.House, Universe = Universe.RealWorld },
      Weapons = weapons,
      DoorKeys = doorKeys,
      MisaHealthUnits = misaHealth,
      KinoHealthUnits = kinoHealth
    };

    SaveGameState(gameState, fileName);
  }

  private GhostStoryGameState LoadGameState(string fileName = "gamestate.xml")
  {
    var filePath = Application.persistentDataPath + "/" + fileName;

    Logger.UnityDebugLog("Loading game state from file " + filePath);
    Logger.Info("Loading game state from file " + filePath);

    if (!File.Exists(filePath))
    {
      Logger.Info("Game state file " + filePath + " does not exist. Creating default");
      CreateDefaultGameState(fileName);
    }

    using (var fileStream = File.Open(filePath, FileMode.Open))
    {
      var serializer = new XmlSerializer(typeof(GhostStoryGameState));

      return (GhostStoryGameState)serializer.Deserialize(fileStream);
    }
  }

  public void SaveGameState(
    GhostStoryGameState gameState,
    string fileName = "gamestate.xml")
  {
    var filePath = Application.persistentDataPath + "/" + fileName;

    Logger.Info("Saving game state file " + filePath);

    using (var memoryStream = new MemoryStream())
    {
      using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
      {
        var serializer = new XmlSerializer(typeof(GhostStoryGameState));

        serializer.Serialize(streamWriter, gameState);

        File.WriteAllBytes(filePath, memoryStream.ToArray());
      }
    }
  }

  public void DisableAndHideAllObjects()
  {
    foreach (var lookupGrouping in _gameObjectsByLayerUniverseKey)
    {
      foreach (var gameObject in lookupGrouping)
      {
        gameObject.DisableAndHide();
      }
    }
  }

  public bool IsRealWorldActivated()
  {
    return GameState.ActiveUniverse.Universe == Universe.RealWorld;
  }

  public bool IsAlternateWorldActivated()
  {
    return GameState.ActiveUniverse.Universe == Universe.AlternateWorld;
  }

  private void DisableCurrentGameObjects()
  {
    foreach (var gameObject in _gameObjectsByLayerUniverseKey[GameState.ActiveUniverse])
    {
      gameObject.DisableAndHide();
    }

    foreach (var freezable in _freezeableGameObjectsByLayerUniverseKey[GameState.ActiveUniverse])
    {
      freezable.Freeze();
    }
  }

  private void EnableCurrentGameObjects()
  {
    foreach (var gameObject in _gameObjectsByLayerUniverseKey[GameState.ActiveUniverse])
    {
      gameObject.EnableAndShow();
    }

    foreach (var freezable in _freezeableGameObjectsByLayerUniverseKey[GameState.ActiveUniverse])
    {
      freezable.Unfreeze();
    }
  }

  private void SwitchUniverse(Universe universe)
  {
    DisableCurrentGameObjects();

    GameState.ActiveUniverse = new LayerUniverseKey { Layer = GameState.ActiveUniverse.Layer, Universe = universe };

    EnableCurrentGameObjects();
  }

  public void SwitchToRealWorld()
  {
    _freezableTimer.Unfreeze();

    SwitchUniverse(Universe.RealWorld);
  }

  public void SwitchToAlternateWorld()
  {
    _freezableTimer.Freeze();

    SwitchUniverse(Universe.AlternateWorld);
  }

  public void SwitchLayer(LevelLayer layer)
  {
    Camera.main.GetComponent<CameraController>().ClearCameraModifiers();

    DisableCurrentGameObjects();

    GameState.ActiveUniverse = new LayerUniverseKey { Layer = layer, Universe = GameState.ActiveUniverse.Universe };

    EnableCurrentGameObjects();
  }

  public void RegisterCallback(float delay, Action action, string name)
  {
    _freezableTimer.RegisterCallback(delay, action, name);
  }

  public void CancelCallback(string name)
  {
    _freezableTimer.CancelCallback(name);
  }
}
