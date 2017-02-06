using System;

[Serializable]
public class CameraSettings
{
  public float HorizontalOffsetDeltaMovementFactor = 0;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

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

      hash = hash * 23 + HorizontalOffsetDeltaMovementFactor.GetHashCode();
      hash = hash * 23 + VerticalCameraFollowMode.GetHashCode();

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
