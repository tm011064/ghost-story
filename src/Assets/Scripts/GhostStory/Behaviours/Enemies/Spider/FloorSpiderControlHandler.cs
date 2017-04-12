using UnityEngine;

public class FloorSpiderControlHandler : AbstractSpiderControlHandler
{
  public FloorSpiderControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController, enemyMovementSettings)
  {
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    CharacterPhysicsManager.WarpToFloor();

    _direction = EnemyMovementSettings.StartDirection;

    EnemyController.Animator.Play("Floor");

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = CharacterPhysicsManager.Velocity
      .SetX(_direction.Multiplier() * EnemyMovementSettings.Speed)
      .SetY(-10);

    var moveCalculationResult = CharacterPhysicsManager.CalculateMove(velocity * Time.deltaTime);

    if (HasCollidedWithLeftWall(moveCalculationResult))
    {
      StepBackFromWall(velocity);

      RegisterLeftWallControlHandler(Direction.Up);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (HasCollidedWithRightWall(moveCalculationResult))
    {
      StepBackFromWall(velocity);

      RegisterRightWallControlHandler(Direction.Up);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    CharacterPhysicsManager.PerformMove(moveCalculationResult);

    if (!moveCalculationResult.CollisionState.Below)
    {
      if (_direction == Direction.Right)
      {
        RegisterLeftWallControlHandler(Direction.Down);
      }
      else
      {
        RegisterRightWallControlHandler(Direction.Down);
      }

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
