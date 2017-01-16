using UnityEngine;

public class FreezeRealWorldPlayerControlHandler : FreezePlayerControlHandler
{
  private readonly XYAxisState _frozenAxisState;

  public FreezeRealWorldPlayerControlHandler(
    PlayerController playerController,
    WorldSwitchSettings worldSwitchSettings)
    : base(
      playerController,
      worldSwitchSettings.Duration,
      Animator.StringToHash("Freeze"),
      new PlayerState[] { PlayerState.Locked, PlayerState.Invincible })
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

    GameManager.ActivatePlayer(
      PlayableCharacterNames.Misa.ToString(),
      GameManager.GetPlayerByName(PlayableCharacterNames.Misa.ToString()).transform.position);

    base.Dispose();
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (GameManager.InputStateManager.IsButtonDown("Switch")
      && GhostStoryGameContext.Instance.IsAlternateWorldActivated())
    {
      var player = GameManager.GetPlayerByName(PlayableCharacterNames.Misa.ToString());
      player.transform.position = GameManager.Player.transform.position;

      GameManager.InputStateManager.SetButtonHandled("Switch");

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return base.DoUpdate();
  }
}
