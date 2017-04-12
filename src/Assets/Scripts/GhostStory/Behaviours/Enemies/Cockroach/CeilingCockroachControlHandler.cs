using UnityEngine;

public class CeilingCockroachControlHandler : BaseControlHandler
{
  private readonly MovingEnemyController _enemyController;

  private readonly EnemyMovementSettings _enemyMovementSettings;

  private Direction _direction;

  private float _finishSpurtTime;

  public CeilingCockroachControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController.CharacterPhysicsManager)
  {
    _enemyController = enemyController;
    _enemyMovementSettings = enemyMovementSettings;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    _enemyController.Animator.Play("Flip To Back");
    CharacterPhysicsManager.WarpToCeiling();

    _direction = _enemyMovementSettings.StartDirection;
    _finishSpurtTime = Time.time + Random.Range(.1f, .6f);

    _enemyController.AdjustHorizontalSpriteScale(_direction);

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (HasFinishedSpurt())
    {
      RegisterFallControlHandler();
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    var velocity = CalculateVelocity();

    var moveCalculationResult = CharacterPhysicsManager.CalculateMove(velocity * Time.deltaTime);

    if (HasCollidedWithWall(moveCalculationResult))
    {
      StepBackFromWall(velocity);

      RegisterFallControlHandler();
      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    CharacterPhysicsManager.PerformMove(moveCalculationResult);
    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }

  private void RegisterFallControlHandler()
  {
    var fallingControlHandler = new FallingCockroachControlHandler(_enemyController, _enemyMovementSettings);
    _enemyController.InsertControlHandlerBeforeCurrent(fallingControlHandler);
  }

  private void StepBackFromWall(Vector3 velocity)
  {
    _direction = _direction.Opposite();

    velocity.x = _direction.Multiplier() * _enemyMovementSettings.Speed;

    CharacterPhysicsManager.Move(velocity * Time.deltaTime);
  }

  private Vector3 CalculateVelocity()
  {
    var velocity = CharacterPhysicsManager.Velocity;

    velocity.x = _direction.Multiplier() * _enemyMovementSettings.Speed;
    velocity.y = 0f;

    return velocity;
  }

  private bool HasCollidedWithWall(MoveCalculationResult moveCalculationResult)
  {
    return (_direction == Direction.Left && moveCalculationResult.CollisionState.Left)
      || (_direction == Direction.Right && moveCalculationResult.CollisionState.Right);
  }

  private bool HasFinishedSpurt()
  {
    return Time.time >= _finishSpurtTime;
  }
}
