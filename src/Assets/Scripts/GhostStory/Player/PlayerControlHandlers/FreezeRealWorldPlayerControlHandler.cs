using UnityEngine;

public class FreezeRealWorldPlayerControlHandler : FreezePlayerControlHandler
{
  private readonly XYAxisState _frozenAxisState;

  public FreezeRealWorldPlayerControlHandler(PlayerController playerController, float duration)
    : base(
      playerController,
      duration,
      Animator.StringToHash("Freeze"))
  {
    _frozenAxisState = base.GetAxisState();
  }

  protected override XYAxisState GetAxisState()
  {
    return _frozenAxisState;
  }

  public override void Dispose()
  {
    GhostStoryGameContext.Instance.SwitchToRealWorld();

    GameManager.Player.DisableAndHide();

    GameManager.ActivatePlayer(PlayableCharacterNames.Misa.ToString());

    base.Dispose();
  }
}
