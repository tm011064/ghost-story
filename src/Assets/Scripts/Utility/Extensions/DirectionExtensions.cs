using System;
using UnityEngine;

public static class DirectionExtensions
{
  public static Direction RotateAntiClockwise(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Right: return Direction.Up;
      case Direction.Down: return Direction.Right;
      case Direction.Left: return Direction.Down;
      case Direction.Up: return Direction.Left;
    }

    throw new NotSupportedException();
  }

  public static Direction RotateClockwise(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Right: return Direction.Down;
      case Direction.Down: return Direction.Left;
      case Direction.Left: return Direction.Up;
      case Direction.Up: return Direction.Right;
    }

    throw new NotSupportedException();
  }

  public static Direction Opposite(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Right: return Direction.Left;
      case Direction.Down: return Direction.Up;
      case Direction.Left: return Direction.Right;
      case Direction.Up: return Direction.Down;
    }

    throw new NotSupportedException();
  }

  public static float Multiplier(this Direction direction)
  {
    switch (direction)
    {
      case Direction.Down:
      case Direction.Left:
        return -1;
    }

    return 1;
  }

  public static Vector2 ToVector(this Direction direction)
  {
    return new Vector2(
      direction == Direction.Left || direction == Direction.Right ? direction.Multiplier() : 0,
      direction == Direction.Up || direction == Direction.Down ? direction.Multiplier() : 0);
  }

  public static Direction Update(this Direction self, Vector3 vector)
  {
    return (
      vector.x > 0
        ? Direction.Right
        : vector.x < 0
          ? Direction.Left
          : (self | Direction.Right) != 0 ? Direction.Right : Direction.Left)
      | (
        vector.y > 0
          ? Direction.Up
          : vector.y < 0
            ? Direction.Down
            : (self | Direction.Up) != 0 ? Direction.Up : Direction.Down);
  }

  public static string ToDirectionString(this Direction self)
  {
    return ((self & Direction.Left) != 0 ? "Left" : "Right")
      + ((self & Direction.Up) != 0 ? " Up" : " Down");
  }
}
