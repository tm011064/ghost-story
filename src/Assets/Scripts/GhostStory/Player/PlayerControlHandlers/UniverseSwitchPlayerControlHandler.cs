
public class UniverseSwitchPlayerControlHandler : DefaultPlayerControlHandler
{
  private readonly WorldSwitchSettings _worldSwitchSettings;

  public UniverseSwitchPlayerControlHandler(
    PlayerController playerController,
    WorldSwitchSettings worldSwitchSettings)
    : base(playerController)
  {
    _worldSwitchSettings = worldSwitchSettings;
    GhostStoryGameContext.Instance.SwitchToRealWorld();
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (GameManager.InputStateManager.IsButtonDown("Switch"))
    {
      var position = GameManager.Player.transform.position;

      GameManager.Player.PushControlHandler(
        new FreezeRealWorldPlayerControlHandler(
          GameManager.Player,
          _worldSwitchSettings));

      GameManager.ActivatePlayer(PlayableCharacterNames.Kino.ToString(), position);

      GameManager.Player.EnableAndShow();
      GameManager.Player.SpawnLocation = position;
      GameManager.Player.Respawn();

      GhostStoryGameContext.Instance.SwitchToAlternateWorld();
    }

    return base.DoUpdate();
  }
}
