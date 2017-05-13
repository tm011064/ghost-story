using UnityEngine;

public class FreezeRealWorldSwitchPortalControlHandler : FreezePlayerControlHandler
{
  private readonly XYAxisState _frozenAxisState;

  public FreezeRealWorldSwitchPortalControlHandler(
    PlayerController playerController)
    : base(
      playerController,
      -1,
      Animator.StringToHash("Freeze"),
      new PlayerState[] { PlayerState.Locked, PlayerState.Invincible })
  {
    _frozenAxisState = base.GetAxisState();
  }

  protected override XYAxisState GetAxisState()
  {
    return _frozenAxisState;
  }
}
