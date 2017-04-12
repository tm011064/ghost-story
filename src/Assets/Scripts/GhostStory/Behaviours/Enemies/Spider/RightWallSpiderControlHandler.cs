using UnityEngine;

public class RightWallSpiderControlHandler : AbstractSpiderControlHandler
{
  public RightWallSpiderControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController, enemyMovementSettings)
  {
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    CharacterPhysicsManager.WarpToRightWall();

    _direction = EnemyMovementSettings.StartDirection;

    EnemyController.Animator.Play("Right Wall");

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = CharacterPhysicsManager.Velocity
      .SetX(10)
      .SetY(_direction.Multiplier() * EnemyMovementSettings.Speed);

    var moveCalculationResult = CharacterPhysicsManager.CalculateMove(velocity * Time.deltaTime);

    if (HasCollidedWithCeiling(moveCalculationResult))
    {
      StepBackFromCeilingOrFloor(velocity);

      RegisterCeilingControlHandler(Direction.Left);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (HasCollidedWithFloor(moveCalculationResult))
    {
      StepBackFromCeilingOrFloor(velocity);

      RegisterFloorControlHandler(Direction.Left);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    CharacterPhysicsManager.PerformMove(moveCalculationResult);

    if (!moveCalculationResult.CollisionState.Right)
    {
      if (_direction == Direction.Up)
      {
        RegisterFloorControlHandler(Direction.Right);
      }
      else
      {
        RegisterCeilingControlHandler(Direction.Right);
      }

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
