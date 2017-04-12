using UnityEngine;

public class AbstractSpiderControlHandler : BaseControlHandler
{
  protected readonly MovingEnemyController EnemyController;

  protected readonly EnemyMovementSettings EnemyMovementSettings;

  protected Direction _direction;

  protected AbstractSpiderControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController.CharacterPhysicsManager)
  {
    EnemyController = enemyController;
    EnemyMovementSettings = enemyMovementSettings;    
  }

  protected void StepBackFromWall(Vector3 velocity)
  {
    _direction = _direction.Opposite();

    velocity.x = _direction.Multiplier() * EnemyMovementSettings.Speed;

    CharacterPhysicsManager.Move(velocity * Time.deltaTime);
  }

  protected void StepBackFromCeilingOrFloor(Vector3 velocity)
  {
    _direction = _direction.Opposite();

    velocity.y = _direction.Multiplier() * EnemyMovementSettings.Speed;

    CharacterPhysicsManager.Move(velocity * Time.deltaTime);
  }

  protected bool HasCollidedWithRightWall(MoveCalculationResult moveCalculationResult)
  {
    return _direction == Direction.Right
      && moveCalculationResult.CollisionState.Right;
  }

  protected bool HasCollidedWithLeftWall(MoveCalculationResult moveCalculationResult)
  {
    return _direction == Direction.Left
      && moveCalculationResult.CollisionState.Left;
  }

  protected bool HasCollidedWithCeiling(MoveCalculationResult moveCalculationResult)
  {
    return _direction == Direction.Up
      && moveCalculationResult.CollisionState.Above;
  }

  protected bool HasCollidedWithFloor(MoveCalculationResult moveCalculationResult)
  {
    return _direction == Direction.Down
      && moveCalculationResult.CollisionState.Below;
  }

  protected void RegisterLeftWallControlHandler(Direction direction)
  {
    EnemyController.InsertControlHandlerBeforeCurrent(
      new LeftWallSpiderControlHandler(
        EnemyController,
        EnemyMovementSettings.Clone(direction)));
  }

  protected void RegisterRightWallControlHandler(Direction direction)
  {
    EnemyController.InsertControlHandlerBeforeCurrent(
      new RightWallSpiderControlHandler(
        EnemyController,
        EnemyMovementSettings.Clone(direction)));
  }

  protected void RegisterCeilingControlHandler(Direction direction)
  {
    EnemyController.InsertControlHandlerBeforeCurrent(
      new CeilingSpiderControlHandler(
        EnemyController,
        EnemyMovementSettings.Clone(direction)));
  }

  protected void RegisterFloorControlHandler(Direction direction)
  {
    EnemyController.InsertControlHandlerBeforeCurrent(
      new FloorSpiderControlHandler(
        EnemyController,
        EnemyMovementSettings.Clone(direction)));
  }
}