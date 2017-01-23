using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostStorySceneManager : MonoBehaviour, ISceneManager
{
  public string StartScene;

  public string StartScenePortalName;

  public float FadeDuration = 1;

  private GameObject _blackBar;

  private AsyncOperation _sceneLoadOperation;

  void Awake()
  {
    var blackBarGameObject = GameObject.Find("Black Bar");
    _blackBar = blackBarGameObject.transform.FindChild("Canvas").gameObject;

    _blackBar.SetActive(false);
    _blackBar.GetComponent<BlackBarCanvas>().FadeOutCompleted += FadeCompleted;
    _blackBar.GetComponent<BlackBarCanvas>().FadeInCompleted += FadeCompleted;
  }

  void FadeCompleted()
  {
    _blackBar.SetActive(false);
    // _sceneLoadOperation.allowSceneActivation = true;
  }

  public void LoadScene(string sceneName, string portalName)
  {
    _blackBar.SetActive(true);
    _blackBar.GetComponent<BlackBarCanvas>().StartFadeOut(FadeDuration);

    //StartCoroutine(LoadSceneAsync(sceneName));
  }

  IEnumerator LoadSceneAsync(string sceneName)
  {
    _sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    _sceneLoadOperation.allowSceneActivation = false;

    yield return _sceneLoadOperation;
  }

  public void LoadScene()
  {
    GameManager.Instance.Player.transform.position = GameObject.FindObjectsOfType<Checkpoint>().First().transform.position;

    _blackBar.SetActive(true);
    _blackBar.GetComponent<BlackBarCanvas>().StartFadeIn(FadeDuration);
  }
}
