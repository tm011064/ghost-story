using UnityEngine;

public static class BehaviourExtensions
{
  public static void DisableAndHide(this Behaviour self)
  {
    self.GetComponentInChildren<SpriteRenderer>().enabled = false;
    self.enabled = false;
  }

  public static void EnableAndShow(this Behaviour self)
  {
    self.GetComponentInChildren<SpriteRenderer>().enabled = true;
    self.enabled = true;
  }
}
