using System;
using System.Linq;
using UnityEngine;

public partial class BoxColliderTriggerEnterBehaviour : ColliderTriggerEnterBehaviour, ITriggerEnterExit
{
  public PlayerState[] PlayerStatesNeededToEnter;

  protected override void InvokeHandler(EventHandler<TriggerEnterExitEventArgs> handler, Collider2D collider)
  {
    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();

    handler(this, new TriggerEnterExitEventArgs(boxCollider, collider));
  }

  protected override bool CanEnter()
  {
    var playerState = GameManager.Instance.Player.PlayerState;

    return PlayerStatesNeededToEnter.All(ps => (playerState & ps) != 0);
  }
}
