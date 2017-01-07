using System.Collections.Generic;
using UnityEngine;

public class Cockroach : MovingEnemyController
{
  public EnemyMovementSettings MovementSettings;

  public Direction[] StartDirections;

  public StartLocation Location;

  public override void OnPlayerCollide(PlayerController playerController)
  {
  }

  public override bool CanSpawn()
  {
    var collider = GetComponent<BoxCollider2D>();

    var bounds = GetBounds(collider);

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
      case Cockroach.StartLocation.Ceiling:
        ResetControlHandlers(new CeilingCockroachControlHandler(this, movementSettings));
        break;

      case Cockroach.StartLocation.Floor:
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
