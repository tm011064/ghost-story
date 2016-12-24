
public class CockroachControlHandler : BaseControlHandler
{
  public CockroachControlHandler(MovingEnemyController enemyController)
    : base(enemyController.CharacterPhysicsManager)
  {
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {


    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
