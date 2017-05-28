using System;
using Assets.Scripts.GhostStory.Behaviours;
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
    GetHouseDoorFadeInBehaviour().MovePlayerToTargetPosition();
  }

  public void SpawnPlayerFromPortal(Vector3 fromPortalPosition)
  {
    GetHouseDoorFadeInBehaviour().StartCameraScroll(fromPortalPosition);
  }

  public bool IsTransitionDoorActive()
  {
    return TransitionDoor.gameObject.activeSelf;
  }

  public string GetPortalName()
  {
    return PortalName;
  }

  public bool HasName(string name)
  {
    return string.Equals(PortalName, name, StringComparison.OrdinalIgnoreCase);
  }

  private HouseDoorFadeInBehaviour GetHouseDoorFadeInBehaviour()
  {
    return GetComponentInChildren<HouseDoorFadeInBehaviour>();
  }

  public bool CanSpawn()
  {
    var config = GetComponent<LevelObjectConfig>();

    return GhostStoryGameContext.Instance.GameState.ActiveUniverse == config.Universe;
  }
}
