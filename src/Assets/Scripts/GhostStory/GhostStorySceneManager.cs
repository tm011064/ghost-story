using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostStorySceneManager : MonoBehaviour, ISceneManager
{
  public string StartScene;

  public string StartScenePortalName;

  public float FadeDuration = 1;

  public GameObject BlackBarPrefab;

  private GameObject _blackBarPrefabInstance;

  private IDontDestroyOnLoad[] _persistedComponents;

  private LoadContext _loadContext = new LoadContext { IsLoading = false };

  void Awake()
  {
    _persistedComponents = gameObject.GetComponents<IDontDestroyOnLoad>();
  }

  public void FadeOut(Action onFadeCompleted = null)
  {
    var blackBarCanvas = GetBlackBarCanvas();

    blackBarCanvas.StartFadeOut(FadeDuration, onFadeCompleted);
  }

  public void FadeIn(Action onFadeCompleted = null)
  {
    var blackBarCanvas = GetBlackBarCanvas();

    blackBarCanvas.StartFadeIn(FadeDuration, onFadeCompleted);
  }

  public void ShowBlackScreen()
  {
    var blackBarCanvas = GetBlackBarCanvas();
    blackBarCanvas.ShowBlackScreen();
  }

  public void LoadScene(string sceneName, string portalName, Vector3 fromPortalPosition)
  {
    var blackBarCanvas = GetBlackBarCanvas();
    StartCoroutine(LoadSceneAsync(sceneName, blackBarCanvas, portalName, fromPortalPosition));
  }

  IEnumerator LoadSceneAsync(
    string sceneName,
    BlackBarCanvas blackBarCanvas,
    string portalName,
    Vector3 fromPortalPosition)
  {
    _loadContext.IsLoading = true;
    _loadContext.PortalName = portalName;
    _loadContext.FromPortalPosition = fromPortalPosition;

    var sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    sceneLoadOperation.allowSceneActivation = false;

    while (blackBarCanvas.IsFading())
    {
      yield return null;
    }

    sceneLoadOperation.allowSceneActivation = true;

    while (!sceneLoadOperation.isDone)
    {
      yield return null;
    }

    OnLoadCompleted(portalName, fromPortalPosition);
  }

  private void OnLoadCompleted(
    string portalName,
    Vector3 fromPortalPosition)
  {
    foreach (var component in _persistedComponents)
    {
      component.OnSceneLoad();
    }

    _loadContext.IsLoading = false;
  }

  private BlackBarCanvas GetBlackBarCanvas()
  {
    if (_blackBarPrefabInstance == null)
    {
      _blackBarPrefabInstance = Instantiate(BlackBarPrefab);
    }

    return _blackBarPrefabInstance.GetComponent<BlackBarCanvas>();
  }

  public bool IsFading()
  {
    var blackBarCanvas = GetBlackBarCanvas();

    return blackBarCanvas.IsFading();
  }

  public bool IsLoading()
  {
    return _loadContext.IsLoading;
  }

  public void OnSceneLoad()
  {
    ActivatePlayer();

    if (!IsLoading())
    {
      SpawnPlayer();

      Camera.main.GetComponent<CameraController>().Reset();
      FocusCameraOnPlayer();

      FadeIn();
    }
    else
    {
      SpawnPlayerFromPortal();
    }
  }

  public void FocusCameraOnPlayer()
  {
    foreach (var cameraModifier in this.FindSceneComponents<CameraModifier>())
    {
      cameraModifier.TryForceTrigger();
    }
  }

  private void SpawnPlayerFromPortal()
  {
    this.FindSceneComponents<IScenePortal>()
      .Single(p => p.HasName(_loadContext.PortalName))
      .SpawnPlayerFromPortal(_loadContext.FromPortalPosition);
  }

  private void SpawnPlayer()
  {
    var portal = GetScenePortalFromGameState();

    portal.SpawnPlayer();
  }

  private IScenePortal GetScenePortalFromGameState()
  {
    var portalName = GetCurrentPortalName();

#if FINAL
    this.FindSceneComponents<IScenePortal>().Single(p => p.HasName(GetCurrentPortalName()));
#else
    return string.IsNullOrEmpty(portalName)
      ? this.FindSceneComponents<IScenePortal>().Where(p => p.CanSpawn()).First() // when started from debugger
      : this.FindSceneComponents<IScenePortal>().Single(p => p.HasName(portalName));
#endif
  }

  private string GetCurrentPortalName()
  {
    return GhostStoryGameContext.Instance.CheckpointManager.HasCheckpoint()
      ? GhostStoryGameContext.Instance.CheckpointManager.CheckpointName
      : GhostStoryGameContext.Instance.GameState.SpawnPlayerPortalName;
  }

  private void ActivatePlayer()
  {
    var playerName = string.IsNullOrEmpty(GhostStoryGameContext.Instance.GameState.SpawnPlayerName)
      ? GameManager.Instance.PlayableCharacters.Single(p => p.IsDefault).PlayerController.name
      : GhostStoryGameContext.Instance.GameState.SpawnPlayerName;

    GameManager.Instance.ActivatePlayer(playerName, Vector3.zero);
  }

  private class LoadContext
  {
    public bool IsLoading;

    public string PortalName;

    public Vector2 FromPortalPosition;
  }
}
