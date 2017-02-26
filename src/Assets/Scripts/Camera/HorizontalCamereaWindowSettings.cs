using System;

[Serializable]
public class HorizontalCamereaWindowSettings
{
  public float WindowWitdh;

  public float DirectionChangeDamping;

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

      hash = hash * 23 + WindowWitdh.GetHashCode();
      hash = hash * 23 + DirectionChangeDamping.GetHashCode();

      return hash;
    }
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }

  public HorizontalCamereaWindowSettings Clone()
  {
    return MemberwiseClone() as HorizontalCamereaWindowSettings;
  }
}
