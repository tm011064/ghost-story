﻿using UnityEngine;

public static class Vector3Extensions
{
  public static Vector3 SetX(this Vector3 vector, float x)
  {
    return new Vector3(
      x,
      vector.y,
      vector.z);
  }

  public static Vector2 SetX(this Vector2 vector, float x)
  {
    return new Vector2(
      x,
      vector.y);
  }
}
