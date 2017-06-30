using UnityEngine;

public class FlashDart : AbstractWeaponBehaviour, IWeapon
{
  public override string Name { get { return "FlashDart"; } }

  private bool _isAttacking;

  protected string _attackAnimation;

  public ProjectileWeaponSettings Settings;

  private float _lastBulletTime = float.MinValue;

  protected override void OnAwake()
  {
    ObjectPoolingManager.Instance.RegisterPoolOrThrow(
      Settings.ProjectilePrefab,
      Settings.MaximumSimultaneouslyActiveProjectiles,
      Settings.MaximumSimultaneouslyActiveProjectiles);
  }

  protected Vector2 GetDirectionVector(XYAxisState axisState)
  {
    if (GameManager.Instance.InputStateManager.IsButtonPressed("Right Shoulder"))
    {
      return new Vector2(GetHorizontalDirection(axisState), 1);
    }

    if (GameManager.Instance.InputStateManager.IsButtonPressed("Left Shoulder"))
    {
      return new Vector2(GetHorizontalDirection(axisState), -1);
    }

    if (axisState.IsInVerticalSensitivityDeadZone())
    {
      return new Vector2(GetHorizontalDirection(axisState), 0);
    }

    if (axisState.IsInHorizontalSensitivityDeadZone())
    {
      return new Vector2(0, GetVerticalDirection(axisState));
    }

    return new Vector2(GetHorizontalDirection(axisState), GetVerticalDirection(axisState));
  }

  private float GetVerticalDirection(XYAxisState axisState)
  {
    return axisState.YAxis > 0 ? 1 : -1;
  }

  private float GetHorizontalDirection(XYAxisState axisState)
  {
    return (axisState.IsInHorizontalSensitivityDeadZone() && Player.IsFacingRight()) || axisState.XAxis > 0f
        ? 1
        : -1;
  }

  public override PlayerStateUpdateResult UpdatePlayerState(XYAxisState axisState)
  {
    if (CanFire(axisState))
    {
      var direction = GetDirectionVector(axisState);

      var spawnLocation = GetSpawnLocation(direction);

      var projectile = ObjectPoolingManager.Instance.GetObject(
        Settings.ProjectilePrefab.name,
        spawnLocation);

      if (projectile != null)
      {
        var projectileBehaviour = projectile.GetComponent<PlayerProjectileBehaviour>();

        projectileBehaviour.StartMove(
          spawnLocation,
          direction * Settings.DistancePerSecond);

        _lastBulletTime = Time.time;

        _attackAnimation = GetAttackAnimation(axisState);
        _isAttacking = true;

        return PlayerStateUpdateResult.CreateHandled(_attackAnimation, 1);
      }
    }

    if ((Player.PlayerState & PlayerState.EnemyContactKnockback) == 0
      && IsAttacking())
    {
      return PlayerStateUpdateResult.CreateHandled(GetAttackAnimation(axisState), 1);
    }

    return PlayerStateUpdateResult.Unhandled;
  }

  private Vector2 GetSpawnLocation(Vector2 direction)
  {
    var spawnLocation = Player.IsGrounded()
      ? Settings.GroundedSpawnLocation
      : Settings.AirborneSpawnLocation;

    return new Vector2(
      direction.x > 0f
        ? Player.transform.position.x + spawnLocation.x
        : Player.transform.position.x - spawnLocation.x
      , Player.transform.position.y + spawnLocation.y);
  }

  private bool IsWithinRateOfFire()
  {
    return _lastBulletTime + (1f / Settings.MaxProjectilesPerSecond) <= Time.time;
  }

  private bool CanFire(XYAxisState axisState)
  {
    return (Player.PlayerState & PlayerState.EnemyContactKnockback) == 0
      && (Settings.EnableAutomaticFire
        ? GameManager.Instance.InputStateManager.IsButtonPressed(Settings.InputButtonName)
        : GameManager.Instance.InputStateManager.IsButtonDown(Settings.InputButtonName))
      && IsWithinRateOfFire();
  }

  public override void StopAttack()
  {
    _isAttacking = false;
    _attackAnimation = null;
  }

  public override bool IsAttacking()
  {
    return _isAttacking;
  }

  private string GetAttackAnimation(XYAxisState axisState)
  {
    if (axisState.IsDown())
    {
      return "Shoot Down";
    }

    if (axisState.IsUp())
    {
      return "Shoot Up";
    }

    return "Shoot";
  }
}
