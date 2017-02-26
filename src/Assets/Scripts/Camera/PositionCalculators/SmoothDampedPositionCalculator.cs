using UnityEngine;

public class SmoothDampedPositionCalculator
{
  private float _lastDirection;

  private float _smoothDampVelocity;

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
}
