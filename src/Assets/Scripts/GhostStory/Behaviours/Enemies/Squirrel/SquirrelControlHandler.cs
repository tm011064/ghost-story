public class SquirrelControlHandler : BaseControlHandler
{
  private readonly EnemyController _enemyController;

  private Direction _direction;

  public SquirrelControlHandler(
    EnemyController enemyController,
    Direction direction)
    : base(enemyController.CharacterPhysicsManager)
  {
    _enemyController = enemyController;
    _direction = direction;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    _enemyController.AdjustHorizontalSpriteScale(_direction);

    _enemyController.Animator.Play("Attack");

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
