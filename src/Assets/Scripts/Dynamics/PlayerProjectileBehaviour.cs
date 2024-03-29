﻿using UnityEngine;

public class PlayerProjectileBehaviour : MonoBehaviour
{
  public int DamageUnits = 1;

  [Tooltip("Specifies what happens to a projectile if an enemy blocks the shot")]
  public ProjectileBlockedBehaviour ProjectileBlockedBehaviour = ProjectileBlockedBehaviour.Disappear;

  private Vector3 _velocity;

  private IProjectileReboundBehaviour _projectileReboundBehaviour;

  private Animator _animator;

  private SpriteRenderer _sprite;

  void Awake()
  {
    _animator = GetComponent<Animator>();
    _sprite = GetComponentInChildren<SpriteRenderer>();

    if (ProjectileBlockedBehaviour == ProjectileBlockedBehaviour.Rebound)
    {
      _projectileReboundBehaviour = this.GetComponentOrThrow<IProjectileReboundBehaviour>();
    }
  }

  public void StartMove(Vector2 startPosition, Vector2 velocity)
  {
    transform.position = startPosition;

    _velocity = velocity.ToVector3();

    var zRotation = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
    _sprite.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);

    if (_animator != null)
    {
      _animator.Play("Emitted");
    }
  }

  void Update()
  {
    transform.Translate(_velocity * Time.deltaTime, Space.World);
  }

  void OnBecameInvisible()
  {
    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      HandleEnemyCollision(collider);

      return;
    }

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  private void HandleEnemyCollision(Collider2D collider)
  {
    var enemyHealthBehaviour = collider.GetComponentOrThrow<EnemyHealthBehaviour>();

    var damageResult = enemyHealthBehaviour.ApplyDamage(DamageUnits);

    if (damageResult == DamageResult.Invincible
      && ProjectileBlockedBehaviour == ProjectileBlockedBehaviour.Rebound)
    {
      _velocity = _projectileReboundBehaviour.HandleRebound(
        GetComponent<BoxCollider2D>(),
        collider,
        _velocity);

      return;
    }

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }
}
