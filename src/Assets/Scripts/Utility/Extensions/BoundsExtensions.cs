using UnityEngine;

public static class BoundsExtensions
{
  public static Rect ToRect(this Bounds self)
  {
    return new Rect(self.center, self.size);
  }

  public static bool AreWithinVerticalShaftOf(this Bounds self, Bounds bounds)
  {
    return self.min.x >= bounds.min.x
      && self.max.x <= bounds.max.x;
  }

  public static bool AreAbove(this Bounds self, Bounds bounds)
  {
    return self.min.y > bounds.max.y;
  }

  public static bool AreAboveOrOnEdge(this Bounds self, Bounds bounds)
  {
    float selfBottom = self.min.y;
    float boundsTop = bounds.max.y;

    return selfBottom > boundsTop
      || Mathf.Approximately(selfBottom, boundsTop);
  }

  public static bool AreBelow(this Bounds self, Bounds bounds)
  {
    return self.max.y < bounds.min.y;
  }

  public static bool ContainBottomEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.max.y >= bounds.min.y
      && self.min.y <= bounds.min.y
      && ((bounds.min.x <= self.min.x && bounds.max.x >= self.min.x)
          || (bounds.max.x >= self.max.x && bounds.min.x <= self.max.x));
  }

  public static bool ContainTopEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.max.y >= bounds.max.y
      && self.min.y <= bounds.max.y
      && ((bounds.min.x <= self.min.x && bounds.max.x >= self.min.x)
          || (bounds.max.x >= self.max.x && bounds.min.x <= self.max.x));
  }

  public static bool ContainLeftEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.max.x >= bounds.min.x
      && self.min.x <= bounds.min.x
      && ((bounds.min.y <= self.min.y && bounds.max.y >= self.min.y)
          || (bounds.max.y >= self.max.y && bounds.min.y <= self.max.y));
  }

  public static bool ContainRightEdgeOf(this Bounds self, Bounds bounds)
  {
    return self.max.x >= bounds.max.x
      && self.min.x <= bounds.max.x
      && ((bounds.min.y <= self.min.y && bounds.max.y >= self.min.y)
          || (bounds.max.y >= self.max.y && bounds.min.y <= self.max.y));
  }
}
