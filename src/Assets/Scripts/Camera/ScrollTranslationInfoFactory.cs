using System;
using UnityEngine;

public static class ScrollTranslationInfoFactory
{
  public static ScrollTranslationInfo Create(
    Vector3 currentPosition,
    Vector3 targetPosition,
    FullScreenScrollerTransitionMode transitionMode,
    float duration
    )
  {
    switch (transitionMode)
    {
      case FullScreenScrollerTransitionMode.Direct:
        return CreateDirectTransition(currentPosition, targetPosition, duration);

      case FullScreenScrollerTransitionMode.FirstVerticalThenHorizontal:
        return CreateFirstVerticalThenHorizontalTransitions(currentPosition, targetPosition, duration);
    }

    throw new NotImplementedException();
  }

  private static ScrollTranslationInfo CreateFirstVerticalThenHorizontalTransitions(
    Vector3 currentPosition,
    Vector3 targetPosition,
    float duration)
  {
    if (Mathf.Approximately(currentPosition.y, targetPosition.y)
      || Mathf.Approximately(currentPosition.x, targetPosition.x))
    {
      return CreateDirectTransition(currentPosition, targetPosition, duration);
    }

    var verticalLocation = new Vector3(currentPosition.x, targetPosition.y);

    var distanceFromCurrentToVerticalLocation = Vector3.Distance(currentPosition, verticalLocation);
    var distanceFromVerticalToTargetLocation = Vector3.Distance(verticalLocation, targetPosition);

    var verticalDistancePercentage = distanceFromCurrentToVerticalLocation / distanceFromVerticalToTargetLocation;

    var verticalTransitionDuration = verticalDistancePercentage * duration;

    var translateTransformActions = new TranslateTransformAction[]
    {
      new TranslateTransformAction(
        verticalLocation,
        verticalTransitionDuration,
        EasingType.Linear,
        GameManager.Instance.Easing),

      new TranslateTransformAction(
        targetPosition,
        duration,
        EasingType.Linear,
        GameManager.Instance.Easing),
    };

    return new ScrollTranslationInfo(
      translateTransformActions,
      verticalTransitionDuration,
      GetAxis(targetPosition, verticalLocation));
  }

  private static ScrollTranslationInfo CreateDirectTransition(
    Vector3 currentPosition,
    Vector3 targetPosition,
    float duration)
  {
    var translateTransformAction = new TranslateTransformAction(
      targetPosition,
      duration,
      EasingType.Linear,
      GameManager.Instance.Easing);

    return new ScrollTranslationInfo(
      new TranslateTransformAction[] { translateTransformAction },
      0f,
      GetAxis(currentPosition, targetPosition));
  }

  private static AxisType GetAxis(Vector3 v1, Vector3 v2)
  {
    return Mathf.Approximately(v1.x, v2.x)
      ? AxisType.Vertical
      : AxisType.Horizontal;
  }
}
