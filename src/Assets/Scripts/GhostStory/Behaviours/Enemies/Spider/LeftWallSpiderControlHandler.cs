using UnityEngine;

public class LeftWallSpiderControlHandler : AbstractSpiderControlHandler
{
  public LeftWallSpiderControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController, enemyMovementSettings)
  {
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    CharacterPhysicsManager.WarpToLeftWall();

    _direction = EnemyMovementSettings.StartDirection;

    EnemyController.Animator.Play("Left Wall");

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = CharacterPhysicsManager.Velocity
      .SetX(-10)
      .SetY(_direction.Multiplier() * EnemyMovementSettings.Speed);

    var moveCalculationResult = CharacterPhysicsManager.CalculateMove(velocity * Time.deltaTime);

    if (HasCollidedWithCeiling(moveCalculationResult))
    {
      StepBackFromCeilingOrFloor(velocity);

      RegisterCeilingControlHandler(Direction.Right);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (HasCollidedWithFloor(moveCalculationResult))
    {
      StepBackFromCeilingOrFloor(velocity);

      RegisterFloorControlHandler(Direction.Right);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    CharacterPhysicsManager.PerformMove(moveCalculationResult);

    if (!moveCalculationResult.CollisionState.Left)
    {
      if (_direction == Direction.Up)
      {
        RegisterFloorControlHandler(Direction.Left);
      }
      else
      {
        RegisterCeilingControlHandler(Direction.Left);
      }

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
