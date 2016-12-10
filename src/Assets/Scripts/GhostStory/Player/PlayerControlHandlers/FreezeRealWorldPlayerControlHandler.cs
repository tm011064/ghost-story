using UnityEngine;

public class FreezeRealWorldPlayerControlHandler : FreezePlayerControlHandler
{
  public FreezeRealWorldPlayerControlHandler(PlayerController playerController)
    : base(
      playerController,
      3, // TODO (Roman): get this from a config value
      Animator.StringToHash("Freeze"))
  {
  }

  public override void Dispose()
  {
    GameManager.Player.GetComponentInChildren<SpriteRenderer>().enabled = false; // TODO (Roman): refactor this
    GameManager.Player.enabled = false;

    GameManager.ActivatePlayer(PlayableCharacterNames.Misa.ToString());

    base.Dispose();
  }
}
