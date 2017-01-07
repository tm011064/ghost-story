using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance = null;

  [HideInInspector]
  public PlayerController Player;

  public PlayableCharacter[] PlayableCharacters;

  [HideInInspector]
  public GameSettings GameSettings;

  [HideInInspector]
  public InputStateManager InputStateManager;

  public InputSettings InputSettings = new InputSettings();

  [HideInInspector]
  public Easing Easing;

  private Checkpoint[] _orderedSceneCheckpoints;

  private CameraController _cameraController;

  private readonly Dictionary<string, PlayerController> _playerControllersByName
    = new Dictionary<string, PlayerController>(StringComparer.OrdinalIgnoreCase);

  private int _currentCheckpointIndex = 0;

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

  public void SpawnPlayerAtNextCheckpoint(bool doCycle)
  {
    if (_currentCheckpointIndex >= _orderedSceneCheckpoints.Length - 1)
    {
      if (doCycle)
      {
        _currentCheckpointIndex = 0;
      }
      else
      {
        _currentCheckpointIndex = _orderedSceneCheckpoints.Length - 1;
      }
    }
    else
    {
      _currentCheckpointIndex++;
    }

    var checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

    Player.SpawnLocation = checkpoint.gameObject.transform.position;

    Player.Respawn();
  }

  public void SpawnPlayerAtCheckpoint(int checkpointIndex)
  {
    if (checkpointIndex < 0)
    {
      _currentCheckpointIndex = 0;
    }
    else if (checkpointIndex >= _orderedSceneCheckpoints.Length)
    {
      _currentCheckpointIndex = _orderedSceneCheckpoints.Length - 1;
    }
    else
    {
      _currentCheckpointIndex = checkpointIndex;
    }

    var checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

    Player.SpawnLocation = checkpoint.gameObject.transform.position;

    Player.Respawn();
  }

  public void RefreshScene(Vector3 cameraPosition)
  {
    ObjectPoolingManager.Instance.DeactivateAll();

    ResetCameraPosition(cameraPosition);

    foreach (ISceneResetable sceneResetable in FindComponents<ISceneResetable>())
    {
      sceneResetable.OnSceneReset();
    }

#if !FINAL
    _fpsRenderer.SceneStartTime = Time.time;
#endif
  }

  private void ResetCameraPosition(Vector3 position)
  {
    _cameraController.MoveCameraToTargetPosition(position);
  }

  public void LoadScene()
  {
    GameObject checkpoint;

    switch (SceneManager.GetActiveScene().name)
    {
      default:

        _orderedSceneCheckpoints = GameObject.FindObjectsOfType<Checkpoint>();

        Array.Sort(_orderedSceneCheckpoints, (a, b) => a.Index.CompareTo(b.Index));

        _currentCheckpointIndex = 0;

        checkpoint = _orderedSceneCheckpoints[_currentCheckpointIndex].gameObject;

        break;
    }

    ResetPooledObjects();

    ActivatePlayer(
      PlayableCharacters.Single(p => p.IsDefault).PlayerController.name,
      checkpoint.transform.position);

#if !FINAL
    _fpsRenderer.SceneStartTime = Time.time;
#endif
  }

  public void ActivatePlayer(string name, Vector3 position)
  {
    Player = GetPlayerController(name);
    Player.transform.position = position;

    _cameraController.Target = Player.transform;
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

  private IEnumerable<T> FindComponents<T>()
    where T : class
  {
    var monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();

    for (var i = 0; i < monoBehaviours.Length; i++)
    {
      var component = monoBehaviours[i] as T;

      if (component != null)
      {
        yield return component;
      }
    }
  }

  private void ResetPooledObjects()
  {
    var objectPoolingManager = ObjectPoolingManager.Instance;

    objectPoolingManager.DeactivateAndClearAll();

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

    InputStateManager = new InputStateManager();

    InputStateManager.InitializeButtons(
      "Attack",
      "Dash",
      "Fall",
      "Jump",
      "Menu Select",
      "Menu Exit",
      "Pause",
      "Switch"
      ); // TODO (Roman): move this somewhere else

    InputStateManager.InitializeAxes("Horizontal", "Vertical");

    Easing = new Easing();

    DontDestroyOnLoad(gameObject);

    _cameraController = Camera.main.GetComponent<CameraController>();

    OnAwake();
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

  void Update()
  {
    InputStateManager.Update();

    // TODO (Roman): this must not make it into release
    if (Input.GetKey("escape"))
    {
      Logger.Info("quit");

      Application.Quit();
    }

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
