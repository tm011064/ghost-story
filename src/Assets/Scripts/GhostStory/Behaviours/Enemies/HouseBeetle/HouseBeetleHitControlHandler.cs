using UnityEngine;

public class HouseBeetleHitControlHandler : BaseControlHandler
{
  private readonly HouseBeetle _enemy;

  private readonly SpriteRenderer _sprite;

  private readonly PendulumCalculator _pendulumCalculator = new PendulumCalculator(1, .3f, 1, 8);

  private Color _originalColor;

  public HouseBeetleHitControlHandler(HouseBeetle enemy, float duration)
    : base(null, duration)
  {
    _enemy = enemy;
    _sprite = _enemy.GetComponentInChildren<SpriteRenderer>();
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    ResetOverrideEndTime(Duration);

    _originalColor = _sprite.color;

    _enemy.EnemyHealthBehaviour.MakeInvincible();

    return true;
  }

  public override void OnControlHandlerDisposed()
  {
    _sprite.color = _originalColor;
    _enemy.EnemyHealthBehaviour.MakeVulnerable();
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    var colorValue = _pendulumCalculator.Add(Time.deltaTime);

    _sprite.color = new Color(1, colorValue, 1, 1);

    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
