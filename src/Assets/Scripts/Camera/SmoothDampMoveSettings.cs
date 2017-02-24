using System;
using UnityEngine;

[Serializable]
public class SmoothDampMoveSettings
{
  [Tooltip("Camera smooth damping on horizontal character movement.")]
  public float HorizontalSmoothDampTime = .2f;

  [Tooltip("Camera smooth damping on vertical character movement.")]
  public float VerticalSlowSmoothDampTime = .2f;

  [Tooltip("Camera smooth damping on rapid descents. For example, if the player falls down at high speeds, we want the camera to stay tight so the player doesn't move off screen.")]
  public float VerticalFastSmoothDampTime = .01f;

  public override bool Equals(object obj)
  {
    return obj != null
      && GetHashCode() == obj.GetHashCode();
  }

  public override int GetHashCode()
  {
    unchecked
    {
      int hash = 17;

      hash = hash * 23 + HorizontalSmoothDampTime.GetHashCode();
      hash = hash * 23 + VerticalSlowSmoothDampTime.GetHashCode();
      hash = hash * 23 + VerticalFastSmoothDampTime.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public SmoothDampMoveSettings Clone()
  {
    return MemberwiseClone() as SmoothDampMoveSettings;
  }
}
