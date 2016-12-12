using UnityEngine;

public class FreezeRealWorldPlayerControlHandler : FreezePlayerControlHandler
{
  private readonly XYAxisState _frozenAxisState;

  private readonly WorldSwitchSettings _worldSwitchSettings;

  public FreezeRealWorldPlayerControlHandler(
    PlayerController playerController,
    WorldSwitchSettings worldSwitchSettings)
    : base(
      playerController,
      worldSwitchSettings.Duration,
      Animator.StringToHash("Freeze"))
  {
    _frozenAxisState = base.GetAxisState();
    _worldSwitchSettings = worldSwitchSettings;
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

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return base.DoUpdate();
  }
}
