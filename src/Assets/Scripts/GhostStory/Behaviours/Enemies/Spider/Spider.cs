using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spider : GhostStoryMovingEnemyController
{
  public EnemyMovementSettings MovementSettings;

  public Direction[] StartDirections;

  public Direction Location;
  
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
    Location = options.GetString("StartLocation").ToEnum<Direction>();
    StartDirections = options.GetString("StartDirections").ToManyEnums<Direction>();

    var controlHandler = CreateControlHandler();

    ResetControlHandlers(controlHandler);
  }

  private BaseControlHandler CreateControlHandler()
  {
    var startDirection = StartDirections[Random.Range(0, StartDirections.Length)];
    var movementSettings = MovementSettings.Clone(startDirection);

    switch (Location)
    {
      case Direction.Up:
        return new CeilingSpiderControlHandler(this, movementSettings);

      case Direction.Down:
        return new FloorSpiderControlHandler(this, movementSettings);

      case Direction.Left:
        return new LeftWallSpiderControlHandler(this, movementSettings);

      case Direction.Right:
        return new RightWallSpiderControlHandler(this, movementSettings);
    }

    throw new NotSupportedException(Location.ToString());
  }
}
