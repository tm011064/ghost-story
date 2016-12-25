using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
  private GameObject _canvas;

  private bool _isPaused;

  private Text _resumeText;

  private Text _pausedText;

  private string _selectedItem;

  void Awake()
  {
    _canvas = transform.FindChild("Canvas").gameObject;

    _resumeText = _canvas.transform.FindChild("Resume Text").GetComponent<Text>();
    _pausedText = _canvas.transform.FindChild("Paused Text").GetComponent<Text>();
  }

  void Start()
  {
    _canvas.SetActive(false);
    _isPaused = false;
  }

  void Update()
  {
    if (GameManager.Instance.InputStateManager.IsButtonDown("Pause"))
    {
      if (_isPaused)
      {
        _isPaused = false;
        _canvas.SetActive(false);
        Time.timeScale = 1f;
      }
      else
      {
        _isPaused = true;
        _canvas.SetActive(true);
        Time.timeScale = 0f;
      }
    }

    if (_isPaused)
    {
      if (GameManager.Instance.InputStateManager.IsButtonDown("Switch"))
      {
        if (_selectedItem == "Resume")
        {
          _isPaused = false;
          _canvas.SetActive(false);
          Time.timeScale = 1f;
        }
      }

      if (Input.GetAxisRaw("Vertical") < 0f)
      {
        _selectedItem = "Paused";
        _pausedText.color = new Color(1, .6f, 0);
        _resumeText.color = new Color(1, 1, 1);
      }

      if (Input.GetAxisRaw("Vertical") > 0f)
      {
        _selectedItem = "Resume";
        _pausedText.color = new Color(1, 1, 1);
        _resumeText.color = new Color(1, .6f, 0);
      }
    }
  }
}
