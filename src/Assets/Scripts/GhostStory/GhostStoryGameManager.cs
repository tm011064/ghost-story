using UnityEngine;

public class GhostStoryGameManager : GameManager
{
  private GameObject[] _realWorldGameObjects;

  private GameObject[] _alternateWorldGameObjects;

  protected override void OnAwake()
  {
    _realWorldGameObjects = GameObject.FindGameObjectsWithTag("RealWorld");
    _alternateWorldGameObjects = GameObject.FindGameObjectsWithTag("AlternateWorld");
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
