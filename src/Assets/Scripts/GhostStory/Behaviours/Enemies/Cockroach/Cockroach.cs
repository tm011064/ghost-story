using System.Collections.Generic;
using UnityEngine;

public class Cockroach : GhostStoryMovingEnemyController
{
  public EnemyMovementSettings MovementSettings;

  public Direction[] StartDirections;

  public StartLocation Location;

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

    var bounds = collider.GetBoundsWhenDisabled();

    return !bounds.Intersects(GameManager.Instance.Player.EnemyBoxCollider.bounds);
  }

  public override void Reset(IDictionary<string, string> options)
  {
    Location = options.GetString("StartLocation").ToEnum<StartLocation>();
    StartDirections = options.GetString("StartDirections").ToManyEnums<Direction>();

    var startDirection = StartDirections[Random.Range(0, StartDirections.Length)];
    var movementSettings = MovementSettings.Clone(startDirection);

    switch (Location)
    {
      case StartLocation.Ceiling:
        ResetControlHandlers(new CeilingCockroachControlHandler(this, movementSettings));
        break;

      case StartLocation.Floor:
        ResetControlHandlers(new FloorCockroachControlHandler(this, movementSettings));
        break;
    }
  }

  public enum StartLocation
  {
    Ceiling,
    Floor
  }
}
