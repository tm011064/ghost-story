﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance = null;

  [HideInInspector]
  public PlayerController Player;

  public PlayableCharacter[] PlayableCharacters;

  [HideInInspector]
  public ISceneManager SceneManager;

  [HideInInspector]
  public GameSettings GameSettings;

  [HideInInspector]
  public InputStateManager InputStateManager;

  public InputSettings InputSettings = new InputSettings();

  private readonly Dictionary<string, PlayerController> _playerControllersByName
    = new Dictionary<string, PlayerController>(StringComparer.OrdinalIgnoreCase);

#if !FINAL
  private readonly FPSRenderer _fpsRenderer = new FPSRenderer();
#endif

  public IEnumerable<PlayerController> GetPlayerControllers()
  {
    return _playerControllersByName.Values;
  }

  public PlayerController GetPlayerByName(string name)
  {
    return _playerControllersByName[name];
  }

  public void LoadScene()
  {
    BuildPooledObjects();

    _playerControllersByName.Clear();
    // TODO (Important): on scene loads, player can move or attack prior to load animation end
    SceneManager.OnSceneLoad();

#if !FINAL
    _fpsRenderer.SceneStartTime = Time.time;
#endif
  }

  public void ActivatePlayer(string name, Vector3 position)
  {
    Player = GetPlayerController(name);
    Player.transform.position = position;
    Player.gameObject.SetActive(true);

    Camera.main.GetComponent<CameraController>().Target = Player.transform;
  }

  private PlayerController GetPlayerController(string name)
  {
    PlayerController playerController;
    if (!_playerControllersByName.TryGetValue(name, out playerController))
    {
      var prefab = PlayableCharacters.Single(
        p => string.Equals(p.PlayerController.name, name, StringComparison.OrdinalIgnoreCase));

      playerController = Instantiate(
        prefab.PlayerController,
        Vector3.zero,
        Quaternion.identity) as PlayerController;

      _playerControllersByName[name] = playerController;
    }

    return playerController;
  }

  private void BuildPooledObjects()
  {
    var objectPoolingManager = ObjectPoolingManager.Instance;

    objectPoolingManager.Reset();

    var monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();

    var gameObjectTypes = new Dictionary<string, ObjectPoolRegistrationInfo>();

    for (var i = 0; i < monoBehaviours.Length; i++)
    {
      var objectPoolBehaviour = monoBehaviours[i] as IObjectPoolBehaviour;
      if (objectPoolBehaviour == null)
      {
        continue;
      }

      foreach (var objectPoolRegistrationInfo in objectPoolBehaviour.GetObjectPoolRegistrationInfos())
      {
        ObjectPoolRegistrationInfo existingObjectPoolRegistrationInfo;
        if (gameObjectTypes.TryGetValue(objectPoolRegistrationInfo.GameObject.name, out existingObjectPoolRegistrationInfo)
          && existingObjectPoolRegistrationInfo.TotalInstances >= objectPoolRegistrationInfo.TotalInstances)
        {
          continue;
        }

        gameObjectTypes[objectPoolRegistrationInfo.GameObject.name] = objectPoolRegistrationInfo.Clone();
      }
    }

    Logger.Info("Registering " + gameObjectTypes.Count + " objects at object pool.");

    foreach (ObjectPoolRegistrationInfo objectPoolRegistrationInfo in gameObjectTypes.Values)
    {
      objectPoolingManager.RegisterPool(
        objectPoolRegistrationInfo.GameObject,
        objectPoolRegistrationInfo.TotalInstances,
        int.MaxValue);
    }
  }

  void Awake()
  {
    Logger.Info("Awaking Game Manager at " + DateTime.Now.ToString());

    if (Instance == null)
    {
      Logger.Info("Setting Game Manager instance.");

      Instance = this;
    }
    else if (Instance != this)
    {
      Logger.Info("Destroying Game Manager instance.");

      Destroy(gameObject);
    }

    if (PlayableCharacters == null || !PlayableCharacters.Any())
    {
      throw new InvalidOperationException("GameManager must have at least one playable character defined");
    }

    if (!PlayableCharacters.Any(p => p.IsDefault)
      || PlayableCharacters.Count(p => p.IsDefault) > 1)
    {
      throw new InvalidOperationException("GameManager must have exactly one default playable character defined");
    }

    SceneManager = gameObject.GetComponentOrThrow<ISceneManager>();

    InitializeInputStateManager();

    DontDestroyOnLoad(gameObject);

    OnAwake();
  }

  void InitializeInputStateManager()
  {
    var buttonNames = new string[]
    {
      "Attack",
      "Dash",
      "Fall",
      "Jump",
      "Menu Select",
      "Menu Exit",
      "Menu Debug Toggle Available",
      "Pause",
      "Right Shoulder",
      "Left Shoulder",
      "Switch"
    };

#if DEBUG
    buttonNames.Concat(new string[] 
      {
        "Menu Debug Toggle Available"
      });
#endif

    InputStateManager = new InputStateManager();

    InputStateManager.InitializeButtons(buttonNames); // TODO (old): move this somewhere else

    InputStateManager.InitializeAxes("Horizontal", "Vertical");
  }

  protected virtual void OnAwake()
  {
  }

  void Start()
  {
    OnStart();
  }

  protected virtual void OnStart()
  {
  }

#if !FINAL
  private DebugUpdateActionManager _debugUpdateActionManager = new DebugUpdateActionManager();

  class DebugUpdateActionManager
  {
    //
    private int _portalIndex;

    public void Update()
    {
      // TODO (Important): this must not make it into release
      if (Input.GetKey("escape"))
      {
        Logger.Info("quit");

        Application.Quit();
      }

      if (Input.GetKeyUp("]"))
      {
        LoadNextPortal();
      }
      if (Input.GetKeyUp("["))
      {
        LoadPreviousPortal();
      }
    }

    private int Repeat(int value, int length)
    {
      if (value >= length)
      {
        return 0;
      }

      if (value < 0)
      {
        return length - 1;
      }

      return value;
    }

    private void LoadPortal(int indexIncrement)
    {
      var portals = GameManager.Instance.FindSceneComponents<IScenePortal>().ToArray();

      _portalIndex = Repeat(_portalIndex + indexIncrement, portals.Length);

      GhostStoryGameContext.Instance.GameState.SpawnPlayerPortalName = portals[_portalIndex].GetPortalName();

      Logger.Info("Loading from portal " + GhostStoryGameContext.Instance.GameState.SpawnPlayerPortalName);

      GameManager.Instance.SceneManager.OnSceneLoad();
    }

    private void LoadPreviousPortal()
    {
      LoadPortal(-1);
    }

    private void LoadNextPortal()
    {
      LoadPortal(1);
    }
  }
#endif

  void Update()
  {
    InputStateManager.Update();

#if !FINAL
    _debugUpdateActionManager.Update();
#endif

#if !FINAL
    _fpsRenderer.UpdateFPS();
#endif
  }

  void OnDestroy()
  {
    Logger.Destroy();
  }

#if !FINAL
  void OnGUI()
  {
    _fpsRenderer.RenderFPS();
  }
#endif
}
