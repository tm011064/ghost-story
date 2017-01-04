using UnityEngine;

public class FloorCockroachControlHandler : BaseControlHandler
{
  private readonly MovingEnemyController _enemyController;

  private readonly EnemyMovementSettings _enemyMovementSettings;

  private float _nextSpurtStartTime;

  private bool _isSpurting;

  private Direction _direction;

  private bool _shouldTurnAroundOnNextSpurt;

  public FloorCockroachControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController.CharacterPhysicsManager)
  {
    _enemyController = enemyController;
    _enemyMovementSettings = enemyMovementSettings;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    StopSpurt();

    CharacterPhysicsManager.WarpToFloor();

    _direction = _enemyMovementSettings.StartDirection;
    _shouldTurnAroundOnNextSpurt = false;

    _enemyController.AdjustSpriteScale(_direction);

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    if (ShouldStartSpurt())
    {
      StartSpurt();
    }

    if (!_isSpurting)
    {
      return ControlHandlerAfterUpdateStatus.KeepAlive;
    }

    var velocity = CalculateVelocity();

    var moveCalculationResult = CharacterPhysicsManager.CalculateMove(velocity * Time.deltaTime);

    if (HasCollidedWithWall(moveCalculationResult))
    {
      StopSpurt();

      RegisterTurnOnNextSpurt();

      StepBackFromWall(velocity);

      return ControlHandlerAfterUpdateStatus.KeepAlive;
    }

    if (LostGround(moveCalculationResult))
    {
      CharacterPhysicsManager.PerformMove(moveCalculationResult);

      RegisterFallControlHandler();

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    CharacterPhysicsManager.PerformMove(moveCalculationResult);
    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }

  private void RegisterFallControlHandler()
  {
    var movementSettings = _enemyMovementSettings.Clone(
      _direction == Direction.Right ? Direction.Right : Direction.Left);

    var fallingControlHandler = new FallingCockroachControlHandler(_enemyController, movementSettings);
    _enemyController.InsertControlHandlerBeforeCurrent(fallingControlHandler);

    _enemyController.InsertControlHandlerBeforeCurrent(new StaticEnemyControlHandler(.3f));
  }

  private bool LostGround(MoveCalculationResult moveCalculationResult)
  {
    return !moveCalculationResult.CollisionState.Below;
  }

  private void StepBackFromWall(Vector3 velocity)
  {
    velocity.x = _direction.Multiplier() * _enemyMovementSettings.Speed;
    CharacterPhysicsManager.Move(velocity * Time.deltaTime);
  }

  private void RegisterTurnOnNextSpurt()
  {
    _direction = _direction.Opposite();
    _shouldTurnAroundOnNextSpurt = true;
  }

  private void TurnAround()
  {
    _enemyController.FlipSpriteHorizontally();
  }

  private Vector3 CalculateVelocity()
  {
    var velocity = CharacterPhysicsManager.Velocity;

    velocity.x = _direction.Multiplier() * _enemyMovementSettings.Speed;

    velocity.y = CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
      ? _enemyMovementSettings.Gravity * Time.deltaTime
      : velocity.y + _enemyMovementSettings.Gravity * Time.deltaTime;

    return velocity;
  }

  private bool HasCollidedWithWall(MoveCalculationResult moveCalculationResult)
  {
    return (_direction == Direction.Left && moveCalculationResult.CollisionState.Left)
      || (_direction == Direction.Right && moveCalculationResult.CollisionState.Right);
  }

  private void StartSpurt()
  {
    _enemyController.Animator.Play("Flip To Feet");

    if (_shouldTurnAroundOnNextSpurt)
    {
      TurnAround();
    }

    _isSpurting = true;
  }

  private void StopSpurt()
  {
    _nextSpurtStartTime = Time.time + Random.Range(2f, 4f);
    _isSpurting = false;
  }

  private bool ShouldStartSpurt()
  {
    return !_isSpurting && Time.time > _nextSpurtStartTime;
  }
}
