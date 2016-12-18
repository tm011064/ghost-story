using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class FrontDoor : MonoBehaviour
  {
    public LevelLayer TransitionsToLayer;

    private bool _isPlayerWithinBoundingBox;

    void Update()
    {
      if (_isPlayerWithinBoundingBox
        && !GameManager.Instance.InputStateManager.IsVerticalAxisHandled()
        && GameManager.Instance.InputStateManager.IsUpAxisButtonDown(GameManager.Instance.Player.InputSettings))
      {
        GhostStoryGameContext.Instance.SwitchLayer(TransitionsToLayer);
      }
    }

    void OnEnable()
    {
      _isPlayerWithinBoundingBox = false;
    }

    void OnDisable()
    {
      _isPlayerWithinBoundingBox = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
      _isPlayerWithinBoundingBox = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
      _isPlayerWithinBoundingBox = false;
    }
  }
}
