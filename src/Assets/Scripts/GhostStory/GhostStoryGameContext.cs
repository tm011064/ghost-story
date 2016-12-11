using UnityEngine;

public class GhostStoryGameContext
{
  public static GhostStoryGameContext Instance;

  private GameObject[] _realWorldGameObjects;

  private GameObject[] _alternateWorldGameObjects;

  public GhostStoryGameContext(
    GameObject[] realWorldGameObjects,
    GameObject[] alternateWorldGameObjects)
  {
    _realWorldGameObjects = realWorldGameObjects;
    _alternateWorldGameObjects = alternateWorldGameObjects;
  }

  public void SwitchToRealWorld()
  {
    foreach (var gameObject in _alternateWorldGameObjects)
    {
      gameObject.DisableAndHide();
    }

    foreach (var gameObject in _realWorldGameObjects)
    {
      gameObject.EnableAndShow();
    }
  }

  public void SwitchToAlternateWorld()
  {
    foreach (var gameObject in _realWorldGameObjects)
    {
      gameObject.DisableAndHide();
    }

    foreach (var gameObject in _alternateWorldGameObjects)
    {
      gameObject.EnableAndShow();
    }
  }
}
