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

  private ILookup<Universe, GameObject> _gameObjectsByUniverse;

  private ILookup<Universe, IFreezable> _freezeableGameObjectsByUniverse;

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
    UpdateGameState(LoadGameState());

    DisableAndHideAllObjects();

    SwitchUniverse(GameState.ActiveUniverse);
  }

  public void OnSceneLoad()
  {
    Reset();

    NotifyGameStateChanged();

    DisableAndHideAllObjects();

    SwitchUniverse(GameState.ActiveUniverse);
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

    _gameObjectsByUniverse = items
      .Where(i => i.FreezableComponent == null)
      .ToLookup(c => c.Key, c => c.GameObject);

    _freezeableGameObjectsByUniverse = items
      .Where(i => i.FreezableComponent != null)
      .ToLookup(c => c.Key, c => c.FreezableComponent);
  }

  private void UpdateGameState(GhostStoryGameState gameState)
  {
    GameState = gameState;

    NotifyGameStateChanged();
  }

  private IEnumerable<Universe> CreateKeys(LevelObjectConfig levelObjectConfig)
  {
    if (levelObjectConfig.Universe == Universe.Global)
    {
      yield return Universe.AlternateWorld;
      yield return Universe.RealWorld;

      yield break;
    }

    yield return levelObjectConfig.Universe;
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
    var kinoHealth = GameManager.Instance.GetPlayerByName(PlayableCharacterNames.Kino.ToString()).Health.HealthUnits;

    var gameState = new GhostStoryGameState
    {
      ActiveUniverse = Universe.RealWorld,
      Weapons = weapons,
      DoorKeys = doorKeys,
      MisaHealthUnits = misaHealth,
      KinoHealthUnits = kinoHealth,
      SpawnPlayerName = PlayableCharacterNames.Misa.ToString()
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
    foreach (var lookupGrouping in _gameObjectsByUniverse)
    {
      foreach (var gameObject in lookupGrouping)
      {
        gameObject.DisableAndHide();
      }
    }
  }

  public bool IsRealWorldActivated()
  {
    return GameState.ActiveUniverse == Universe.RealWorld;
  }

  public bool IsAlternateWorldActivated()
  {
    return GameState.ActiveUniverse == Universe.AlternateWorld;
  }

  public void DisableCurrentGameObjects()
  {
    foreach (var gameObject in _gameObjectsByUniverse[GameState.ActiveUniverse])
    {
      gameObject.DisableAndHide();
    }

    foreach (var freezable in _freezeableGameObjectsByUniverse[GameState.ActiveUniverse])
    {
      freezable.Freeze();
    }
  }

  public void EnableCurrentGameObjects()
  {
    foreach (var gameObject in _gameObjectsByUniverse[GameState.ActiveUniverse])
    {
      gameObject.EnableAndShow();
    }

    foreach (var freezable in _freezeableGameObjectsByUniverse[GameState.ActiveUniverse])
    {
      freezable.Unfreeze();
    }
  }

  private void SwitchUniverse(Universe universe)
  {
    DisableCurrentGameObjects();

    GameState.ActiveUniverse = universe;

    EnableCurrentGameObjects();

    Camera.main.GetComponent<CameraController>().Reset(); // TODO (Roman): is that correct? camera moves for some reason
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

  public void RegisterCallback(float delay, Action action, string name)
  {
    _freezableTimer.RegisterCallback(delay, action, name);
  }

  public void CancelCallback(string name)
  {
    _freezableTimer.CancelCallback(name);
  }
}
