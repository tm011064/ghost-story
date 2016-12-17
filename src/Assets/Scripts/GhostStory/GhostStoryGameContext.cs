using UnityEngine;

public class GhostStoryGameContext
{
  public static GhostStoryGameContext Instance;

  private GameObject[] _realWorldGameObjects;

  private GameObject[] _alternateWorldGameObjects;

  private bool _isRealWorld;

  public GhostStoryGameContext(
    GameObject[] realWorldGameObjects,
    GameObject[] alternateWorldGameObjects)
  {
    _realWorldGameObjects = realWorldGameObjects;
    _alternateWorldGameObjects = alternateWorldGameObjects;
  }

  public bool IsRealWorldActivated()
  {
    return _isRealWorld;
  }

  public bool IsAlternateWorldActivated()
  {
    return !_isRealWorld;
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

    _isRealWorld = true;
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

    _isRealWorld = false;
  }
}
