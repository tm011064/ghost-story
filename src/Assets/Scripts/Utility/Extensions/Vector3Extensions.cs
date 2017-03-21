using UnityEngine;

public static class Vector3Extensions
{
  public static Vector3 SetX(this Vector3 vector, float x)
  {
    return new Vector3(
      x,
      vector.y,
      vector.z);
  }

  public static Vector3 SetY(this Vector3 vector, float y)
  {
    return new Vector3(
      vector.x,
      y,
      vector.z);
  }
  
  public static Vector3 SetZ(this Vector3 vector, float z)
  {
    return new Vector3(
      vector.x,
      vector.y,
      z);
  }
  
  public static Vector3 AddX(this Vector3 vector, float value)
  {
    return new Vector3(
      vector.x + value,
      vector.y,
      vector.z);
  }

  public static Vector3 AddY(this Vector3 vector, float value)
  {
    return new Vector3(
      vector.x,
      vector.y + value,
      vector.z);
  }

  public static Vector2 SetX(this Vector2 vector, float x)
  {
    return new Vector2(
      x,
      vector.y);
  }
}
