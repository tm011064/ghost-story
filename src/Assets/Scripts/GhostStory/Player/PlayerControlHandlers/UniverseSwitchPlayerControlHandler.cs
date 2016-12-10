using UnityEngine;

public class UniverseSwitchPlayerControlHandler : DefaultPlayerControlHandler
{
  public UniverseSwitchPlayerControlHandler(PlayerController playerController)
    : base(playerController)
  {
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (GameManager.InputStateManager.IsButtonDown("Switch"))
    {
      var position = GameManager.Player.transform.position;

      GameManager.Player.PushControlHandler(new FreezeRealWorldPlayerControlHandler(GameManager.Player));

      GameManager.ActivatePlayer(PlayableCharacterNames.Kino.ToString(), position);

      GameManager.Player.GetComponentInChildren<SpriteRenderer>().enabled = true; // TODO (Roman): refactor this
      GameManager.Player.enabled = true;
      GameManager.Player.SpawnLocation = position;
      GameManager.Player.Respawn();
    }

    return base.DoUpdate();
  }
}
