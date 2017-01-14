using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerTranslationActionContextFactory
{
  public static IEnumerable<PlayerTranslationActionContext> Create(
    Vector3 currentPosition,
    Vector3 targetPosition,
    FullScreenScrollerTransitionMode transitionMode,
    int playerAnimationShortHash,
    FullScreenScrollSettings fullScreenScrollSettings)
  {
    switch (transitionMode)
    {
      case FullScreenScrollerTransitionMode.Direct:
        return new PlayerTranslationActionContext[]
        { 
          CreateMovingPlayerAction(
            currentPosition,
            targetPosition,
            playerAnimationShortHash,
            fullScreenScrollSettings.TransitionTime,
            fullScreenScrollSettings.PlayerTranslationDistance,
            fullScreenScrollSettings.PlayerTranslationEasingType)
        };

      case FullScreenScrollerTransitionMode.FirstVerticalThenHorizontal:
        return CreateFirstVerticalThenHorizontalTransitions(
          currentPosition,
          targetPosition,
          playerAnimationShortHash,
          fullScreenScrollSettings);
    }

    throw new NotImplementedException();
  }

  private static IEnumerable<PlayerTranslationActionContext> CreateFirstVerticalThenHorizontalTransitions(
    Vector3 currentPosition,
    Vector3 targetPosition,
    int playerAnimationShortHash,
    FullScreenScrollSettings fullScreenScrollSettings)
  {
    if (Mathf.Approximately(currentPosition.y, targetPosition.y)
      || Mathf.Approximately(currentPosition.x, targetPosition.x))
    {
      yield return CreateMovingPlayerAction(
        currentPosition,
        targetPosition,
        playerAnimationShortHash,
        fullScreenScrollSettings.TransitionTime,
        fullScreenScrollSettings.PlayerTranslationDistance,
        fullScreenScrollSettings.PlayerTranslationEasingType);

      yield break;
    }

    var verticalLocation = new Vector3(currentPosition.x, targetPosition.y, targetPosition.z);

    var distanceFromCurrentToVerticalLocation = Vector3.Distance(currentPosition, verticalLocation);
    var distanceFromVerticalToTargetLocation = Vector3.Distance(verticalLocation, targetPosition);

    var verticalDistancePercentage = distanceFromCurrentToVerticalLocation / distanceFromVerticalToTargetLocation;

    var verticalTransitionDuration = verticalDistancePercentage * fullScreenScrollSettings.TransitionTime * 2f;

    yield return CreateStaticPlayerAction(
      currentPosition,
      verticalLocation,
      playerAnimationShortHash,
      verticalTransitionDuration);

    yield return CreateMovingPlayerAction(
      verticalLocation,
      targetPosition,
      playerAnimationShortHash,
      fullScreenScrollSettings.TransitionTime,
      fullScreenScrollSettings.PlayerTranslationDistance,
      EasingType.Linear);
  }

  private static PlayerTranslationActionContext CreateStaticPlayerAction(
    Vector3 currentPosition,
    Vector3 targetPosition,
    int playerAnimationShortHash,
    float duration)
  {
    var translateTransformAction = new TranslateTransformAction(
      targetPosition,
      duration,
      EasingType.Linear,
      GameManager.Instance.Easing);

    var playerControlHandler = new FreezePlayerControlHandler(
        GameManager.Instance.Player,
        duration,
        playerAnimationShortHash,
        new PlayerState[] { PlayerState.Locked, PlayerState.Invincible });

    return new PlayerTranslationActionContext
    {
      TranslateTransformAction = translateTransformAction,
      PlayerControlHandler = playerControlHandler
    };
  }

  private static PlayerTranslationActionContext CreateMovingPlayerAction(
    Vector3 currentPosition,
    Vector3 targetPosition,
    int playerAnimationShortHash,
    float duration,
    float distance,
    EasingType easingType)
  {
    var translateTransformAction = new TranslateTransformAction(
      targetPosition,
      duration,
      EasingType.Linear, // TODO (Roman): get this from somewhere?
      GameManager.Instance.Easing);

    var playerControlHandler = CreateMovingPlayerControlHandler(
      currentPosition,
      targetPosition,
      playerAnimationShortHash,
      duration,
      distance,
      easingType);

    return new PlayerTranslationActionContext
    {
      TranslateTransformAction = translateTransformAction,
      PlayerControlHandler = playerControlHandler
    };
  }

  private static PlayerControlHandler CreateMovingPlayerControlHandler(
    Vector3 currentPosition,
    Vector3 targetPosition,
    int playerAnimationShortHash,
    float duration,
    float distance,
    EasingType easingType)
  {
    var playerTranslationVector = GetPlayerTranslationVector(
      targetPosition,
      currentPosition,
      distance);

    return new TranslateFrozenPlayerControlHandler(
      GameManager.Instance.Player,
      duration,
      playerAnimationShortHash,
      playerTranslationVector,
      easingType);
  }

  private static AxisType GetTranslationAxis(Vector3 v1, Vector3 v2)
  {
    return Mathf.Approximately(v1.x, v2.x)
      ? AxisType.Vertical
      : AxisType.Horizontal;
  }

  private static Vector3 GetPlayerTranslationVector(
    Vector3 cameraTargetPosition,
    Vector3 cameraPosition,
    float distance)
  {
    if (distance == 0f)
    {
      return Vector3.zero;
    }

    var translationAxis = GetTranslationAxis(cameraTargetPosition, cameraPosition);

    var directionVector = translationAxis == AxisType.Horizontal
      ? new Vector3(cameraTargetPosition.x - cameraPosition.x, 0f)
      : new Vector3(0f, cameraTargetPosition.y - cameraPosition.y);

    return directionVector.normalized * distance;
  }
}
