using UnityEngine;

public class JumpFloaterPlayerControlHandler : PlayerControlHandler
{
  private FloatStatus _floatStatus;

  private float _originalInAirDamping;

  private bool _wasFloatingLastFrame;

  private KinoFloatSettings _floatSettings;

  public JumpFloaterPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
    _floatSettings = playerController.GetComponentOrThrow<KinoFloatSettings>();

    _originalInAirDamping = playerController.JumpSettings.InAirDamping;
  }

  public override void Dispose()
  {
    PlayerController.AdjustedGravity = PlayerController.JumpSettings.Gravity;
    PlayerController.JumpSettings.InAirDamping = _originalInAirDamping;
  }

  private FloatStatus CalculateFloatStatus(ref Vector3 velocity)
  {
    if (PlayerController.IsGrounded())
    {
      return FloatStatus.CanFloat;
    }

    if (velocity.y >= 0)
    {
      return FloatStatus.CanFloat | FloatStatus.IsInAir;
    }

    var floatStatus = _floatStatus | FloatStatus.IsInAir;

    if (GameManager.Instance.InputStateManager.IsButtonUp("Jump"))
    {
      floatStatus &= ~FloatStatus.CanFloat;
    }

    if (GameManager.Instance.InputStateManager.IsButtonPressed("Jump"))
    {
      floatStatus |= FloatStatus.IsFloating;
    }

    return floatStatus;
  }

  private bool IsFloating(Vector3 velocity)
  {
    return velocity.y < 0
      && (_floatStatus == (FloatStatus.IsInAir | FloatStatus.CanFloat | FloatStatus.IsFloating));
  }

  private float CalculateAdjustedGravity(Vector3 velocity)
  {
    if (!IsFloating(velocity))
    {
      _wasFloatingLastFrame = false;
      return PlayerController.JumpSettings.Gravity;
    }

    if (_wasFloatingLastFrame)
    {
      return Mathf.Lerp(
        PlayerController.AdjustedGravity,
        PlayerController.JumpSettings.Gravity,
        Time.deltaTime * _floatSettings.GravityDecreaseInterpolationFactor);
    }

    _wasFloatingLastFrame = true;

    return _floatSettings.Gravity;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    HandleOneWayPlatformFallThrough();

    var velocity = PlayerController.CharacterPhysicsManager.Velocity;

    velocity.y = GetJumpVerticalVelocity(velocity);
    velocity.x = GetDefaultHorizontalVelocity(velocity);

    _floatStatus = CalculateFloatStatus(ref velocity);

    PlayerController.AdjustedGravity = CalculateAdjustedGravity(velocity);
    if (IsFloating(velocity))
    {
      PlayerController.JumpSettings.InAirDamping = _floatSettings.InAirDamping;
    }

    velocity.y = Mathf.Max(
      GetGravityAdjustedVerticalVelocity(velocity, PlayerController.AdjustedGravity, true),
      PlayerController.JumpSettings.MaxDownwardSpeed);

    PlayerController.CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }

  enum FloatStatus
  {
    IsInAir = 1,
    CanFloat = 2,
    IsFloating = 4
  }
}
