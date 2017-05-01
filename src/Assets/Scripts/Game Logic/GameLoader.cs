using UnityEngine;

public class GameLoader : MonoBehaviour
{
  public GameObject GameManagerPrefab;

  public GameSettings GameSettings;

  void Awake()
  {
#if !UNITY_EDITOR
    Screen.SetResolution(768, 480, true);
#endif

    Logger.Initialize(GameSettings.LogSettings);

    if (GameManager.Instance == null)
    {
      Instantiate(GameManagerPrefab);
    }

    GameManager.Instance.GameSettings = GameSettings;

    // TODO (old): sound placeholder
    //if (SoundManager.instance == null)
    //  Instantiate(soundManager);
  }

  void Start()
  {
    GameManager.Instance.LoadScene();
  }
}