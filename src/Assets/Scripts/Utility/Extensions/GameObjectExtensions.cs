using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameObjectExtensions
{
  public static GameObject GetParentGameObject(this GameObject self)
  {
    if (self == null)
    {
      return null;
    }

    if (self.transform.parent == null)
    {
      return null;
    }

    return self.transform.parent.gameObject;
  }

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

  public static IEnumerable<TComponent> GetComponentsOrThrow<TComponent>(this GameObject self, params Func<TComponent, bool>[] conditions)
  {
    var components = self.GetComponents<TComponent>();

    var untriggeredConditions = conditions.ToList();

    foreach (var component in components)
    {
      foreach (var condition in conditions)
      {
        if (condition(component))
        {
          untriggeredConditions.Remove(condition);

          yield return component;
          break;
        }
      }
    }

    if (untriggeredConditions.Any())
    {
      throw new MissingComponentException("Not all expected components of type '" + typeof(TComponent).ToString() + "' found at game object '" + self.name + "'");
    }
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
    SetActive(self, false);
  }

  public static void EnableAndShow(this GameObject self)
  {
    SetActive(self, true);
  }

  private static void SetActive(GameObject gameObject, bool isActive)
  {
    gameObject.SetActive(isActive);

    var renderer = gameObject.GetComponentInChildren<Renderer>();
    if (renderer != null)
    {
      renderer.enabled = isActive;
    }
  }
}
