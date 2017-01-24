using System.Collections;
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

  void Awake()
  {
    _persistedComponents = gameObject.GetComponents<IDontDestroyOnLoad>();
  }

  public void FadeIn()
  {
    var blackBarCanvas = GetBlackBarCanvas();

    blackBarCanvas.gameObject.SetActive(true);
    blackBarCanvas.StartFadeIn(FadeDuration);
  }

  public void LoadScene(string sceneName, string portalName)
  {
    var blackBarCanvas = GetBlackBarCanvas();

    blackBarCanvas.gameObject.SetActive(true);
    blackBarCanvas.StartFadeOut(FadeDuration);

    StartCoroutine(LoadSceneAsync(sceneName, blackBarCanvas));
  }

  IEnumerator LoadSceneAsync(string sceneName, BlackBarCanvas blackBarCanvas)
  {
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

    foreach (var component in _persistedComponents)
    {
      component.OnSceneLoad();
    }
  }

  private BlackBarCanvas GetBlackBarCanvas()
  {
    if (_blackBarPrefabInstance == null)
    {
      _blackBarPrefabInstance = Instantiate(BlackBarPrefab);
    }

    return _blackBarPrefabInstance
      .transform
      .FindChild("Canvas")
      .gameObject
      .GetComponent<BlackBarCanvas>();
  }
}
