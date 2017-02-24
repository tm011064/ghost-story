using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerTranslationActionContextFactory
{
  public static IEnumerable<PlayerTranslationActionContext> Create(
    Vector3 currentPosition,
    Vector3 targetPosition,
    int playerAnimationShortHash,
    FullScreenScrollSettings fullScreenScrollSettings)
  {
    switch (fullScreenScrollSettings.FullScreenScrollerTransitionMode)
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

    var verticalTranslation = VerticalScrollTranslationCalculator.CalculateVerticalTranslation(
      currentPosition,
      targetPosition,
      fullScreenScrollSettings.TransitionTime,
      fullScreenScrollSettings.VerticalFullScreenScrollerTransitionSpeedFactor);

    yield return CreateStaticPlayerAction(
      currentPosition,
      verticalTranslation.Location,
      playerAnimationShortHash,
      verticalTranslation.Duration);

    yield return CreateMovingPlayerAction(
      verticalTranslation.Location,
      targetPosition,
      playerAnimationShortHash,
      fullScreenScrollSettings.TransitionTime,
      fullScreenScrollSettings.PlayerTranslationDistance,
      fullScreenScrollSettings.PlayerTranslationEasingType);
  }

  private static PlayerTranslationActionContext CreateStaticPlayerAction(
    Vector3 currentPosition,
    Vector3 targetPosition,
    int playerAnimationShortHash,
    float duration)
  {
    var translateTransformAction = new TranslateTransformAction(
      targetPosition,
      duration);

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
    EasingType playerTranslationEasingType)
  {
    var translateTransformAction = new TranslateTransformAction(
      targetPosition,
      duration);

    var playerControlHandler = CreateMovingPlayerControlHandler(
      currentPosition,
      targetPosition,
      playerAnimationShortHash,
      duration,
      distance,
      playerTranslationEasingType);

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
