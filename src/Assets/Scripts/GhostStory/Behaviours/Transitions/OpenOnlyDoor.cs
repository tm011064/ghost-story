using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class OpenOnlyDoor : MonoBehaviour
  {
    public string Name;

    public bool IsOpen = false;

    public DoorKey DoorKey = DoorKey.AlternateDoorKey;

    public Vector2 CameraModifierPadding = new Vector2(6, 6);

    private GameObject _leftDoor;

    private GameObject _rightDoor;

    private GameObject _leftTransitionDoor;

    private GameObject _rightTransitionDoor;

    private BaseControlHandler _freezeControlHandler;

    private DoorTriggerEnterBehaviour _doorTriggerEnterBehaviour;

    void Awake()
    {
      _doorTriggerEnterBehaviour = GetComponentInChildren<DoorTriggerEnterBehaviour>();
      _doorTriggerEnterBehaviour.Open += OpenDoor;

      _leftDoor = transform.Find("Left Door").gameObject;
      _rightDoor = transform.Find("Right Door").gameObject;
      _leftTransitionDoor = transform.Find("Left Transition Door").gameObject;
      _rightTransitionDoor = transform.Find("Right Transition Door").gameObject;
    }

    void OnDestroy()
    {
      _doorTriggerEnterBehaviour.Open -= OpenDoor;
    }

    public void OpenDoor()
    {
      if (IsTransitionDoorActive())
      {
        return;
      }

      IsOpen = true;

      ShowTransitionDoor();
    }

    private void SetWallDoorsActive(bool isActive)
    {
      _leftDoor.SetActive(isActive);
      _rightDoor.SetActive(isActive);
    }

    bool IsTransitionDoorActive()
    {
      return _leftTransitionDoor.activeSelf || _rightTransitionDoor.activeSelf;
    }

    void ShowTransitionDoor()
    {
      SetWallDoorsActive(false);

      var transitionDoor = GameManager.Instance.Player.IsFacingRight()
        ? _leftTransitionDoor
        : _rightTransitionDoor;
      transitionDoor.SetActive(true);
    }

    void HideTransitionDoor()
    {
      SetWallDoorsActive(true);

      _leftTransitionDoor.SetActive(false);
      _rightTransitionDoor.SetActive(false);
    }
  }
}
