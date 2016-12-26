using UnityEngine;

public class Yoyo : MonoBehaviour, IWeapon
{
  public int DamageUnits = 1;

  private bool _isAttacking;

  private bool _canAttack = true;

  private string _attackAnimation;

  private BaseControlHandler _controlHandler;

  private GameManager GameManager;

  void Awake()
  {
    GameManager = GameManager.Instance;
  }

  public void StopAttack()
  {
    if (!_isAttacking)
    {
      return;
    }

    _isAttacking = false;
    _attackAnimation = null;

    GameManager.Player.RemoveControlHandler(_controlHandler);
    _controlHandler = null;

    if (IsPlayerAirborneAndGoingUp())
    {
      StopPlayerUpdwardMovement();
    }
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

  public bool IsAttacking()
  {
    return _isAttacking;
  }

  private string GetAttackAnimation(XYAxisState axisState)
  {
    if (GameManager.Player.IsAirborne()
        && axisState.IsDown())
    {
      return "Yoyo Down";
    }

    if (axisState.IsUp())
    {
      return "Yoyo Up";
    }

    return "Yoyo 180";
  }

  public PlayerStateUpdateResult UpdateState(XYAxisState axisState)
  {
    if (_isAttacking)
    {
      return PlayerStateUpdateResult.CreateHandled(_attackAnimation, 1, allowHorizontalSpriteFlip: false);
    }

    if (_canAttack && GameManager.InputStateManager.IsButtonDown("Attack"))
    {
      if (GameManager.Player.IsAirborne())
      {
        _canAttack = false;
        GameManager.Player.GroundedPlatformChanged += OnGroundedPlatformChanged;
      }

      _isAttacking = true;

      _attackAnimation = GetAttackAnimation(axisState);

      _controlHandler = new FreezePlayerControlHandler(
        GameManager.Player,
        10f,
        Animator.StringToHash(_attackAnimation));

      GameManager.Player.PushControlHandler(_controlHandler);

      return PlayerStateUpdateResult.CreateHandled(_attackAnimation, 1, allowHorizontalSpriteFlip: false);
    }

    return PlayerStateUpdateResult.Unhandled;
  }

  void OnGroundedPlatformChanged(GroundedPlatformChangedInfo obj)
  {
    _canAttack = true;
    GameManager.Player.GroundedPlatformChanged -= OnGroundedPlatformChanged;
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
