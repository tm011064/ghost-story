using UnityEngine;

public abstract class AbstractYoyo : AbstractWeaponBehaviour
{
  public int DamageUnits = 1;

  private bool _isAttacking;

  private bool _canExecuteAirborneAttack = true;

  protected string AttackAnimation;

  protected abstract string GetAttackAnimation(XYAxisState axisState);

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
    return Player.IsAirborne()
      && Player.CharacterPhysicsManager.Velocity.y > 0;
  }

  private void StopPlayerUpdwardMovement()
  {
    Player.CharacterPhysicsManager.Velocity = new Vector3(
      Player.CharacterPhysicsManager.Velocity.x,
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
      return PlayerStateUpdateResult.CreateHandled(AttackAnimation, 1);
    }

    if (Player.IsGrounded())
    {
      _canExecuteAirborneAttack = true;
    }

    if (_canExecuteAirborneAttack && InputStateManager.IsUnhandledButtonDown("Attack"))
    {
      InputStateManager.SetButtonHandled("Attack");

      if (Player.IsAirborne())
      {
        _canExecuteAirborneAttack = false;
      }

      _isAttacking = true;

      AttackAnimation = GetAttackAnimation(axisState);

      OnStartAttack();

      return PlayerStateUpdateResult.CreateHandled(AttackAnimation, 1);
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
