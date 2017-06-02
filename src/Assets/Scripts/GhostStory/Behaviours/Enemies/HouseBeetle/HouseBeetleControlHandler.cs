using UnityEngine;

public class HouseBeetleControlHandler : BaseControlHandler
{
  private readonly HouseBeetle _enemy;

  private readonly EnemyMovementSettings _enemyMovementSettings;

  private Vector3 _velocity;

  private Direction _lastDirection;

  public HouseBeetleControlHandler(
    HouseBeetle enemy,
    EnemyMovementSettings enemyMovementSettings)
    : base(null)
  {
    _enemy = enemy;
    _enemyMovementSettings = enemyMovementSettings;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    var direction = GameManager.Instance.Player.transform.position - _enemy.transform.position;

    _lastDirection = direction.CalculateDirection();

    PlayAnimation(direction);

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var playerDelta = GameManager.Instance.Player.transform.position - _enemy.transform.position;
    var safeDeltaTime = Mathf.Min(Time.deltaTime, .02f);

    var deltaMovement = playerDelta.magnitude <= _enemy.MaxPlayerChaseDistance
      ? playerDelta.normalized * _enemyMovementSettings.Speed * safeDeltaTime
      : playerDelta.normalized * .001f;

    _velocity = _velocity
      .SetX(Mathf.Lerp(_velocity.x, deltaMovement.x, _enemy.DirectionChangeSmoothDampFactor * safeDeltaTime))
      .SetY(Mathf.Lerp(_velocity.y, deltaMovement.y, _enemy.DirectionChangeSmoothDampFactor * safeDeltaTime));

    _enemy.transform.Translate(_velocity);

    PlayAnimation(deltaMovement);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }

  private void PlayAnimation(Vector3 directionVector)
  {
    _lastDirection = _lastDirection.Update(directionVector);

    var animationName = _lastDirection.ToDirectionString();

    _enemy.Animator.Play(animationName);
  }
}
