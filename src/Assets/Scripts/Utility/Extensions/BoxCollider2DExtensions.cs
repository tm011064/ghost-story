using UnityEngine;

public static class BoxCollider2DExtensions
{
  public static Bounds GetBoundsWhenDisabled(this BoxCollider2D collider)
  {
    if (collider.isActiveAndEnabled)
    {
      return collider.bounds;
    }

    var bounds = collider.bounds;

    bounds.size = collider.size;

    return bounds;
  }
}
