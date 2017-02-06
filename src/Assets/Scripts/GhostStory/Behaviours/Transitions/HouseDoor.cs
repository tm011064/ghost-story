using UnityEngine;

public partial class HouseDoor : ScenePortal, IScenePortal
{
  [HideInInspector]
  public GameObject Door;

  [HideInInspector]
  public GameObject TransitionDoor;

  void Awake()
  {
    Door = transform.Find("Door").gameObject;
    TransitionDoor = transform.Find("Transition Door").gameObject;
  }

  public void SpawnPlayer()
  {
    GetHouseDoorFadeInBehaviour().MovePlayerToTargetPosition(this, GameManager.Instance.Player);
  }

  public void SpawnPlayerFromPortal(Vector3 fromPortalPosition)
  {
    GetHouseDoorFadeInBehaviour().StartCameraScroll(fromPortalPosition);
  }

  public string GetPortalName()
  {
    return PortalName;
  }

  private HouseDoorFadeInBehaviour GetHouseDoorFadeInBehaviour()
  {
    return GetComponentInChildren<HouseDoorFadeInBehaviour>();
  }
}
