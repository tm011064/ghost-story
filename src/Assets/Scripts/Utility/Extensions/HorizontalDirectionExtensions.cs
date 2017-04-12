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

  public static HorizontalDirection ToHorizontalDirection(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Right: return HorizontalDirection.Right;
      case Direction.Left: return HorizontalDirection.Left;
    }

    throw new NotSupportedException();
  }

  public static float Multiplier(this HorizontalDirection direction)
  {
    return direction == HorizontalDirection.Left ? -1 : 1;
  }
}
