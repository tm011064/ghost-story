
public class UniverseSwitchPlayerControlHandler : DefaultPlayerControlHandler
{
  private readonly WorldSwitchSettings _worldSwitchSettings;

  public UniverseSwitchPlayerControlHandler(
    PlayerController playerController,
    WorldSwitchSettings worldSwitchSettings)
    : base(playerController)
  {
    _worldSwitchSettings = worldSwitchSettings;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (GameManager.InputStateManager.IsUnhandledButtonDown("Switch")
      && GhostStoryGameContext.Instance.IsRealWorldActivated()
      && GameManager.Player.IsGrounded())
    {
      var position = GameManager.Player.transform.position;

      GameManager.Player.PushControlHandler(
        new FreezeRealWorldPlayerControlHandler(
          GameManager.Player,
          _worldSwitchSettings));

      GameManager.ActivatePlayer(PlayableCharacterNames.Kino.ToString(), position);
      GameManager.Player.EnableAndShow();

      GhostStoryGameContext.Instance.SwitchToAlternateWorld();
    }

    return base.DoUpdate();
  }
}
