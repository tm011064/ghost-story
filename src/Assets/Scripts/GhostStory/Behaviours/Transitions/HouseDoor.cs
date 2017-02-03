using UnityEngine;

public partial class HouseDoor : ScenePortal, IScenePortal
{
  [HideInInspector]
  public GameObject Door;

  [HideInInspector]
  public GameObject TransitionDoor;

  private HouseDoorFadeInBehaviour _houseDoorFadeInBehaviour;

  void Awake()
  {
    Door = transform.Find("Door").gameObject;
    TransitionDoor = transform.Find("Transition Door").gameObject;

    _houseDoorFadeInBehaviour = GetComponentInChildren<HouseDoorFadeInBehaviour>();
  }

  public void SpawnPlayer()
  {
    _houseDoorFadeInBehaviour.FadeIn();
  }

  public void SpawnPlayerFromPortal(Vector3 fromPortalPosition)
  {
    _houseDoorFadeInBehaviour.StartCameraScroll(fromPortalPosition);
  }

  public string GetPortalName()
  {
    return PortalName;
  }
}
