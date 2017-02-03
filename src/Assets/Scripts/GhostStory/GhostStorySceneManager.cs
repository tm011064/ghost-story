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

  private bool _isLoading;

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
    _isLoading = true;

    var currentSceneName = SceneManager.GetActiveScene().name;

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

    this.FindSceneComponents<IScenePortal>()
      .Single(p => p.GetPortalName() == portalName)
      .SpawnPlayerFromPortal(fromPortalPosition);

    _isLoading = false;
  }

  private BlackBarCanvas GetBlackBarCanvas()
  {
    if (_blackBarPrefabInstance == null)
    {
      _blackBarPrefabInstance = Instantiate(BlackBarPrefab);
    }

    return _blackBarPrefabInstance.GetComponent<BlackBarCanvas>();
  }


  public bool IsLoadingSceneTransition()
  {
    return _isLoading;
  }
}
