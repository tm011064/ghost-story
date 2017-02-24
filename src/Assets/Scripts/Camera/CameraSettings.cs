using System;
using UnityEngine;

[Serializable]
public class CameraSettings
{
  public VerticalCameraFollowMode VerticalCameraFollowMode;

  public HorizontalCameraFollowMode HorizontalCameraFollowMode;

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

      hash = hash * 23 + VerticalCameraFollowMode.GetHashCode();
      hash = hash * 23 + HorizontalCameraFollowMode.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public CameraSettings Clone()
  {
    return MemberwiseClone() as CameraSettings;
  }
}
