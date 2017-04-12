using UnityEngine;

public class CeilingSpiderControlHandler : AbstractSpiderControlHandler
{
  public CeilingSpiderControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController, enemyMovementSettings)
  {
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    CharacterPhysicsManager.WarpToCeiling();

    _direction = EnemyMovementSettings.StartDirection;

    EnemyController.Animator.Play("Ceiling");

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = CharacterPhysicsManager.Velocity
      .SetX(_direction.Multiplier() * EnemyMovementSettings.Speed)
      .SetY(10); // TODO (Roman): base this on skin width or something, shouldn't be arbitrary

    var moveCalculationResult = CharacterPhysicsManager.CalculateMove(velocity * Time.deltaTime);

    if (HasCollidedWithLeftWall(moveCalculationResult))
    {
      StepBackFromWall(velocity);

      RegisterLeftWallControlHandler(Direction.Down);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    if (HasCollidedWithRightWall(moveCalculationResult))
    {
      StepBackFromWall(velocity);

      RegisterRightWallControlHandler(Direction.Down);

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    CharacterPhysicsManager.PerformMove(moveCalculationResult);

    if (!moveCalculationResult.CollisionState.Above)
    {
      if (_direction == Direction.Right)
      {
        RegisterLeftWallControlHandler(Direction.Up);
      }
      else
      {
        RegisterRightWallControlHandler(Direction.Up);
      }

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
