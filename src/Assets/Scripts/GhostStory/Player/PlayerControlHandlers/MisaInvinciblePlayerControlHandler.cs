using UnityEngine;

public class MisaInvinciblePlayerControlHandler : DefaultPlayerControlHandler
{
  public MisaInvinciblePlayerControlHandler(
    PlayerController playerController,
    float duration)
    : base(
      playerController,
      duration: duration)
  {
    SetDebugDraw(Color.red, true);
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    PlayerController.PlayerState |= PlayerState.Invincible;
    PlayerController.Animator.SetLayerWeight(PlayerController.Animator.GetLayerIndex("Additive"), 1);
    PlayerController.Animator.Play("Invincible", PlayerController.Animator.GetLayerIndex("Additive"));

    return true;
  }

  public override void Dispose()
  {
    PlayerController.Animator.SetLayerWeight(PlayerController.Animator.GetLayerIndex("Additive"), 0);
    PlayerController.PlayerState &= ~PlayerState.Invincible;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return base.DoUpdate();
  }
}
