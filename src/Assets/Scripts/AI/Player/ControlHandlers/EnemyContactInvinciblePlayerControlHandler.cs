using UnityEngine;

public class EnemyContactInvinciblePlayerControlHandler : DefaultPlayerControlHandler
{
  public EnemyContactInvinciblePlayerControlHandler(
    PlayerController playerController,
    float invincibleDurationAfterKnockback)
    : base(
      playerController,
      duration: invincibleDurationAfterKnockback)
  {
    SetDebugDraw(Color.red, true);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Invincible;

    return true;
  }

  public override void Dispose()
  {
    PlayerController.PlayerState &= ~PlayerState.Invincible;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return base.DoUpdate();
  }
}
