using UnityEngine;

public static class VerticalScrollTranslationCalculator
{
  public static TranslationInfo CalculateVerticalTranslation(
    Vector3 currentPosition,
    Vector3 targetPosition,
    float horizontalScrollDuration,
    float verticalScrollSpeedPercentage)
  {
    var verticalLocation = new Vector3(currentPosition.x, targetPosition.y, targetPosition.z);

    var distanceFromCurrentToVerticalLocation = Vector3.Distance(currentPosition, verticalLocation);
    var distanceFromVerticalToTargetLocation = Vector3.Distance(verticalLocation, targetPosition);

    var horizontalSpeed = horizontalScrollDuration / distanceFromVerticalToTargetLocation;
    var verticalSpeed = horizontalSpeed * verticalScrollSpeedPercentage;

    var verticalTransitionDuration = distanceFromCurrentToVerticalLocation * verticalSpeed;

    return new TranslationInfo
    {
      Location = verticalLocation,
      Duration = verticalTransitionDuration
    };
  }

  public struct TranslationInfo
  {
    public Vector3 Location;

    public float Duration;
  }
}
