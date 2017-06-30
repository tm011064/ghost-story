using UnityEngine;

public static class ComponentExtensions
{
  public static TComponent GetComponentOrThrow<TComponent>(this Component self)
  {
    var component = self.GetComponent<TComponent>();

    if (component == null)
    {
      throw new MissingComponentException("Component of type '" + typeof(TComponent).ToString() + "' missing from game object '" + self.gameObject.name + "'");
    }

    return component;
  }

  public static TComponent FindComponentInParentsOrThrow<TComponent>(this Component self)
  {
    TComponent component = self.GetComponent<TComponent>();
    if (component != null)
    {
      return component;
    }

    var parent = self.transform.parent;
    while (component == null && parent != null)
    {
      component = parent.GetComponent<TComponent>();
      parent = parent.transform.parent;
    }

    if (component == null)
    {
      throw new MissingComponentException("Component of type '" + typeof(TComponent).ToString() + "' missing from game object '" + self.gameObject.name + "'");
    }

    return component;
  }
}
