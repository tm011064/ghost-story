using System;
using System.Collections.Generic;
using UnityEngine;

public static class TranslateTransformActionFactory
{
  public static IEnumerable<TranslateTransformAction> Create(
    Vector3 currentPosition,
    Vector3 targetPosition,
    FullScreenScrollSettings fullScreenScrollSettings)
  {
    switch (fullScreenScrollSettings.FullScreenScrollerTransitionMode)
    {
      case FullScreenScrollerTransitionMode.Direct:
        return new TranslateTransformAction[]
        {  
          new TranslateTransformAction(
            targetPosition,
            fullScreenScrollSettings.TransitionTime)
        };

      case FullScreenScrollerTransitionMode.FirstVerticalThenHorizontal:
        return CreateFirstVerticalThenHorizontalTransitions(
          currentPosition,
          targetPosition,
          fullScreenScrollSettings.TransitionTime,
          fullScreenScrollSettings.VerticalFullScreenScrollerTransitionSpeedFactor);
    }

    throw new NotImplementedException();
  }

  private static IEnumerable<TranslateTransformAction> CreateFirstVerticalThenHorizontalTransitions(
    Vector3 currentPosition,
    Vector3 targetPosition,
    float horizontalScrollDuration,
    float verticalScrollSpeedPercentage)
  {
    if (Mathf.Approximately(currentPosition.y, targetPosition.y)
      || Mathf.Approximately(currentPosition.x, targetPosition.x))
    {
      yield return new TranslateTransformAction(
        targetPosition,
        horizontalScrollDuration);

      yield break;
    }

    var verticalTranslation = VerticalScrollTranslationCalculator.CalculateVerticalTranslation(
      currentPosition,
      targetPosition,
      horizontalScrollDuration,
      verticalScrollSpeedPercentage);

    yield return new TranslateTransformAction(
      verticalTranslation.Location,
      verticalTranslation.Duration);

    yield return new TranslateTransformAction(
      targetPosition,
      horizontalScrollDuration);
  }
}
