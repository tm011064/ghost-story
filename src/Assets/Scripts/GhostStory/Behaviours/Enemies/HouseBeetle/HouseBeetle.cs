using System.Collections.Generic;
using UnityEngine;

public class HouseBeetle : GhostStoryMovingEnemyController
{
  public EnemyMovementSettings MovementSettings;

  public float DirectionChangeSmoothDampFactor = .8f;

  public float MaxPlayerChaseDistance = 120;

  public override void OnPlayerCollide(PlayerController playerController)
  {
    if (GhostStoryGameContext.GameState.ActiveUniverse == EnemyDamageBehaviour.Universe)
    {
      playerController.Health.ApplyDamage(EnemyDamageBehaviour.DamageUnits, EnemyContactReaction.Knockback);
    }
  }

  public override bool CanSpawn()
  {
    var collider = GetComponent<BoxCollider2D>();

    var bounds = GetBounds(collider);

    return !bounds.Intersects(GameManager.Instance.Player.EnemyBoxCollider.bounds);
  }

  public override void Reset(IDictionary<string, string> options)
  {
    var controlHandler = CreateControlHandler();

    ResetControlHandlers(controlHandler);
  }

  private BaseControlHandler CreateControlHandler()
  {
    return new HouseBeetleControlHandler(this, MovementSettings);
  }
}
