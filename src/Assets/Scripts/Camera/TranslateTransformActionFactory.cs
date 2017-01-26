﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class TranslateTransformActionFactory
{
  public static IEnumerable<TranslateTransformAction> Create(
    Vector3 currentPosition,
    Vector3 targetPosition,
    FullScreenScrollerTransitionMode transitionMode,
    float horizontalScrollDuration,
    float verticalScrollSpeedPercentage)
  {
    switch (transitionMode)
    {
      case FullScreenScrollerTransitionMode.Direct:
        return new TranslateTransformAction[]
        {  
          new TranslateTransformAction(
            targetPosition,
            horizontalScrollDuration,
            EasingType.Linear,
            GameManager.Instance.Easing)
        };

      case FullScreenScrollerTransitionMode.FirstVerticalThenHorizontal:
        return CreateFirstVerticalThenHorizontalTransitions(
          currentPosition,
          targetPosition,
          horizontalScrollDuration,
          verticalScrollSpeedPercentage);
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
        horizontalScrollDuration,
        EasingType.Linear,
        GameManager.Instance.Easing);

      yield break;
    }

    var verticalTranslation = VerticalScrollTranslationCalculator.CalculateVerticalTranslation(
      currentPosition,
      targetPosition,
      horizontalScrollDuration,
      verticalScrollSpeedPercentage);

    yield return new TranslateTransformAction(
      verticalTranslation.Location,
      verticalTranslation.Duration,
      EasingType.Linear,
      GameManager.Instance.Easing);

    yield return new TranslateTransformAction(
      targetPosition,
      horizontalScrollDuration,
      EasingType.Linear,
      GameManager.Instance.Easing);
  }
}