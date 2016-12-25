using UnityEngine;

public class FallingCockroachControlHandler : BaseControlHandler
{
  private readonly MovingEnemyController _enemyController;

  private readonly EnemyMovementSettings _enemyMovementSettings;

  public FallingCockroachControlHandler(
    MovingEnemyController enemyController,
    EnemyMovementSettings enemyMovementSettings)
    : base(enemyController.CharacterPhysicsManager)
  {
    _enemyController = enemyController;
    _enemyMovementSettings = enemyMovementSettings;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var velocity = CalculateVelocity();

    CharacterPhysicsManager.Move(velocity * Time.deltaTime);

    if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.BecameGroundedThisFrame)
    {
      _enemyController.Animator.Play("Flip To Back");

      RegisterFloorControlHandler();

      return ControlHandlerAfterUpdateStatus.CanBeDisposed;
    }

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }

  private void RegisterFloorControlHandler()
  {
    var floorControlHandler = new FloorCockroachControlHandler(_enemyController, _enemyMovementSettings);
    _enemyController.InsertControlHandlerBeforeCurrent(floorControlHandler);
  }

  private Vector3 CalculateVelocity()
  {
    var velocity = CharacterPhysicsManager.Velocity;

    velocity.x = 0f;
    velocity.y += _enemyMovementSettings.Gravity * Time.deltaTime;

    return velocity;
  }
}
