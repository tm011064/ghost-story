using UnityEngine;

public class TranslateFrozenPlayerControlHandler : FreezePlayerControlHandler
{
  private TranslateTransformAction _translateTransformAction;

  private readonly Vector3 _playerTranslationVector;

  private readonly EasingType _playerTranslationEasingType;

  public TranslateFrozenPlayerControlHandler(
    PlayerController playerController,
    float suspendPhysicsTime,
    int animationShortNameHash,
    Vector3 playerTranslationVector,
    EasingType playerTranslationEasingType = EasingType.Linear)
    : base(
      playerController,
      suspendPhysicsTime,
      animationShortNameHash,
      new PlayerState[] { PlayerState.Locked, PlayerState.Invincible })
  {
    _playerTranslationVector = playerTranslationVector;
    _playerTranslationEasingType = playerTranslationEasingType;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    Logger.UnityDebugLog(
      "POS", PlayerController.transform.position,
      "NPOS", PlayerController.transform.position + _playerTranslationVector);

    _translateTransformAction = TranslateTransformAction.Start(
      PlayerController.transform.position,
      PlayerController.transform.position + _playerTranslationVector,
      Duration,
      _playerTranslationEasingType);

    return base.TryActivate(previousControlHandler);
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    PlayerController.transform.position = _translateTransformAction.GetPosition();

    Logger.UnityDebugLog("XPOS", PlayerController.transform.position);
    return base.DoUpdate();
  }
}
