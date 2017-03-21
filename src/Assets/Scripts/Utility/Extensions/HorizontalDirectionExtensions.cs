using System;

public static class HorizontalDirectionExtensions
{
  public static HorizontalDirection Opposite(this HorizontalDirection direction)
  {
    switch (direction)
    {
      case HorizontalDirection.Right: return HorizontalDirection.Left;
      case HorizontalDirection.Left: return HorizontalDirection.Right;
    }

    throw new NotSupportedException();
  }
}
