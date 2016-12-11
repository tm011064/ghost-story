using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
  public static void AttachChild(this GameObject self, GameObject child)
  {
    child.transform.parent = self.transform;
  }

  public static TComponent GetComponentOrThrow<TComponent>(this GameObject self)
  {
    var component = self.GetComponent<TComponent>();

    if (component == null)
    {
      throw new MissingComponentException("Component of type '" + typeof(TComponent).ToString() + "' missing from game object '" + self.name + "'");
    }

    return component;
  }

  public static void AttachChildren(this GameObject self, IEnumerable<GameObject> children)
  {
    foreach (var child in children)
    {
      self.AttachChild(child);
    }
  }

  public static void DisableAndHide(this GameObject self)
  {
    Debug.Log("Enabling " + self.name);
    self.GetComponentInChildren<Renderer>().enabled = false;
    self.SetActive(false);
  }

  public static void EnableAndShow(this GameObject self)
  {
    Debug.Log("Disabling " + self.name);
    self.SetActive(true);
    self.GetComponentInChildren<Renderer>().enabled = true;
  }
}
