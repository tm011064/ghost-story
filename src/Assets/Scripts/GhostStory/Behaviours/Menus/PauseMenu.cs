using UnityEngine;

public class PauseMenu : MonoBehaviour
{
  private PauseMenuCanvas _canvas;

  void Awake()
  {
    _canvas = GetComponentInChildren<PauseMenuCanvas>(true);
  }

  void Start()
  {
    _canvas.gameObject.SetActive(false);
  }

  void Update()
  {
    if (GameManager.Instance.InputStateManager.IsButtonDown("Pause"))
    {
      if (!_canvas.gameObject.activeSelf)
      {
        _canvas.gameObject.SetActive(true);
      }
    }
  }
}
