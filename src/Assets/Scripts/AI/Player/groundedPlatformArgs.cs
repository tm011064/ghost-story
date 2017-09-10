using UnityEngine;

public class GroundedPlatformArgs
{
  public GameObject PreviousPlatform;

  public GameObject CurrentPlatform;

  public GroundedPlatformArgs(GameObject previousPlatform, GameObject currentPlatform)
  {
    PreviousPlatform = previousPlatform;
    CurrentPlatform = currentPlatform;
  }
}