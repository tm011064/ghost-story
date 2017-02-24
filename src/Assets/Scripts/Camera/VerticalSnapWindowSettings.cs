using System;

[Serializable]
public class VerticalSnapWindowSettings
{
  public float OuterWindowTopBoundsPositionPercentage;

  public float InnerWindowTopBoundsPositionPercentage;

  public float OuterWindowBottomBoundsPositionPercentage;

  public float InnerWindowBottomBoundsPositionPercentage;

  public float UpwardMovementSnapPositionPercentage;

  public float DownwardMovementSnapPositionPercentage;

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

      hash = hash * 23 + OuterWindowTopBoundsPositionPercentage.GetHashCode();
      hash = hash * 23 + InnerWindowTopBoundsPositionPercentage.GetHashCode();
      hash = hash * 23 + OuterWindowBottomBoundsPositionPercentage.GetHashCode();
      hash = hash * 23 + InnerWindowBottomBoundsPositionPercentage.GetHashCode();
      hash = hash * 23 + UpwardMovementSnapPositionPercentage.GetHashCode();
      hash = hash * 23 + DownwardMovementSnapPositionPercentage.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public VerticalSnapWindowSettings Clone()
  {
    return MemberwiseClone() as VerticalSnapWindowSettings;
  }
}
