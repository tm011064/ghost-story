using System.Reflection;
using UnityEngine;

public class SmoothDampedPositionCalculator
{
  private float _lastDirection;

  private float _smoothDampVelocity;

  public SmoothDampedPositionCalculator(float direction)
  {
    _lastDirection = Mathf.Sign(direction);
  }

  public float CalculatePosition(float from, float to, float smoothDampTime)
  {
    var direction = Mathf.Approximately(from, to)
      ? _lastDirection
      : Mathf.Sign(to - from);

    if (_lastDirection != direction)
    {
      _smoothDampVelocity *= -1;
      _lastDirection = direction;
    }

    return direction == 1
      ? Mathf.SmoothDamp(from, to, ref _smoothDampVelocity, smoothDampTime)
      : from - Mathf.SmoothDamp(0, from - to, ref _smoothDampVelocity, smoothDampTime);
  }

  public SmoothDampedPositionCalculator Clone()
  {
    return (SmoothDampedPositionCalculator)MemberwiseClone();
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted(BindingFlags.Instance | BindingFlags.NonPublic);
  }

  public void Reset()
  {
    _smoothDampVelocity = 0;
  }
}
