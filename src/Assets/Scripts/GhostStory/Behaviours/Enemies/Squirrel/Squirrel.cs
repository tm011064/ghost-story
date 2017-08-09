using System.Collections.Generic;
using UnityEngine;

public class Squirrel : GhostStoryEnemyController, IEnemy
{
  public Direction Direction;

  public ProjectileWeaponSettings Settings;

  public override void Awake()
  {
    base.Awake();

    ObjectPoolingManager.Instance.RegisterOrExpandPool(
      Settings.ProjectilePrefab,
      Settings.MaximumSimultaneouslyActiveProjectiles,
      Settings.MaximumSimultaneouslyActiveProjectiles);
  }

  public override void OnPlayerCollide(PlayerController playerController)
  {
    if (GhostStoryGameContext.GameState.ActiveUniverse == EnemyDamageBehaviour.Universe)
    {
      playerController.Health.ApplyDamage(EnemyDamageBehaviour.DamageUnits, EnemyContactReaction.Knockback);
    }
  }

  public void EmitProjectile()
  {
    var spawnLocation = transform.position;

    GameObject projectile;
    if (ObjectPoolingManager.Instance.TryGetObject(Settings.ProjectilePrefab.name, spawnLocation, out projectile))
    {
      var projectileBehaviour = projectile.GetComponent<EnemyProjectileBehaviour>();

      projectileBehaviour.StartMove(
        spawnLocation,
        Direction.ToVector() * Settings.DistancePerSecond);
    }
  }

  public override bool CanSpawn()
  {
    var collider = GetComponent<BoxCollider2D>();

    var bounds = collider.GetBoundsWhenDisabled();

    return !bounds.Intersects(GameManager.Instance.Player.EnemyBoxCollider.bounds);
  }

  public override void Reset(IDictionary<string, string> options)
  {
    Direction = options.GetString("Direction").ToEnum<Direction>();

    ResetControlHandlers(new SquirrelControlHandler(this, Direction));
  }
}
