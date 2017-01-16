using UnityEngine;

public class EnemyContactKnockbackPlayerControlHandler : PlayerControlHandler
{
  private float _distancePerSecond;

  private float _knockbackDuration;

  private float _knockbackDistance;

  public EnemyContactKnockbackPlayerControlHandler(
    PlayerController playerController,
    float knockbackDuration,
    float knockbackDistance)
    : base(
      playerController,
      new PlayerStateController[] { new EnemyContactKnockbackPlayerStateController(playerController) },
      knockbackDuration)
  {
    SetDebugDraw(Color.red, true);

    _knockbackDuration = knockbackDuration;
    _knockbackDistance = knockbackDistance;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Invincible;
    PlayerController.PlayerState |= PlayerState.EnemyContactKnockback;
    PlayerController.PlayerState |= PlayerState.Locked;

    _distancePerSecond = (1f / _knockbackDuration)
      * _knockbackDistance;

    if (PlayerController.IsFacingRight())
    {
      _distancePerSecond *= -1f;
    }

    return true;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.EnemyContactKnockback;
    PlayerController.PlayerState &= ~PlayerState.Locked;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var deltaMovement = new Vector2(
      Time.deltaTime * _distancePerSecond,
      Mathf.Max(
        GetGravityAdjustedVerticalVelocity(
          PlayerController.CharacterPhysicsManager.Velocity,
          PlayerController.AdjustedGravity,
          true),
        PlayerController.JumpSettings.MaxDownwardSpeed)
      * Time.deltaTime);

    PlayerController.CharacterPhysicsManager.Move(deltaMovement);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
