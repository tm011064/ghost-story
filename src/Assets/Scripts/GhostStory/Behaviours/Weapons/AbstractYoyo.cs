using UnityEngine;

public abstract class AbstractYoyo : AbstractWeaponBehaviour
{
  public int DamageUnits = 1;

  private bool _isAttacking;

  private bool _canAttack = true;

  protected string AttackAnimation;

  protected GameManager GameManager;

  protected abstract string GetAttackAnimation(XYAxisState axisState);

  void Awake()
  {
    GameManager = GameManager.Instance;
  }

  protected virtual void OnStartAttack()
  {
  }

  protected virtual void OnStopAttack()
  {
  }

  public override void StopAttack()
  {
    if (!_isAttacking)
    {
      return;
    }

    _isAttacking = false;
    AttackAnimation = null;

    if (IsPlayerAirborneAndGoingUp())
    {
      StopPlayerUpdwardMovement();
    }

    OnStopAttack();
  }

  private bool IsPlayerAirborneAndGoingUp()
  {
    return GameManager.Player.IsAirborne()
      && GameManager.Player.CharacterPhysicsManager.Velocity.y > 0;
  }

  private void StopPlayerUpdwardMovement()
  {
    GameManager.Player.CharacterPhysicsManager.Velocity = new Vector3(
      GameManager.Player.CharacterPhysicsManager.Velocity.x,
      0f);
  }

  public override bool IsAttacking()
  {
    return _isAttacking;
  }

  public override PlayerStateUpdateResult UpdateState(XYAxisState axisState)
  {
    if (_isAttacking)
    {
      return PlayerStateUpdateResult.CreateHandled(AttackAnimation, 1, allowHorizontalSpriteFlip: false);
    }

    if (GameManager.Player.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.BecameGroundedThisFrame)
    {
      _canAttack = true;
    }

    if (_canAttack && GameManager.InputStateManager.IsButtonDown("Attack"))
    {
      if (GameManager.Player.IsAirborne())
      {
        _canAttack = false;
      }

      _isAttacking = true;

      AttackAnimation = GetAttackAnimation(axisState);

      OnStartAttack();

      return PlayerStateUpdateResult.CreateHandled(AttackAnimation, 1, allowHorizontalSpriteFlip: false);
    }

    return PlayerStateUpdateResult.Unhandled;
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (_isAttacking)
    {
      HandleEnemyCollision(collider);
    }
  }

  private void HandleEnemyCollision(Collider2D collider)
  {
    var enemyHealthBehaviour = collider.GetComponentOrThrow<EnemyHealthBehaviour>();

    enemyHealthBehaviour.ApplyDamage(DamageUnits);
  }
}
