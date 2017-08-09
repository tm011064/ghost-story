using UnityEngine;

public class EnemyProjectileBehaviour : MonoBehaviour, IEnemy
{
  public int DamageUnits = 1;

  [Tooltip("Specifies what happens to a projectile if an enemy blocks the shot")]
  public ProjectileBlockedBehaviour ProjectileBlockedBehaviour = ProjectileBlockedBehaviour.Disappear;

  private bool _deactivateOnceInvisible;

  private Vector3 _velocity;

  private IProjectileReboundBehaviour _projectileReboundBehaviour;

  private Animator _animator;

  private SpriteRenderer _sprite;

  public virtual void Awake()
  {
    _animator = GetComponent<Animator>();
    _sprite = GetComponentInChildren<SpriteRenderer>();

    if (ProjectileBlockedBehaviour == ProjectileBlockedBehaviour.Rebound)
    {
      _projectileReboundBehaviour = this.GetComponentOrThrow<IProjectileReboundBehaviour>();
    }
  }

  public virtual void OnEnable()
  {
    _deactivateOnceInvisible = false;
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

  public virtual void Update()
  {
    transform.Translate(_velocity * Time.deltaTime, Space.World);

    if (_deactivateOnceInvisible && !_sprite.isVisible)
    {
      ObjectPoolingManager.Instance.Deactivate(gameObject);
    }
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
    {
      HandlePlayerCollision(collider);

      return;
    }

    ObjectPoolingManager.Instance.Deactivate(gameObject);
  }

  private void HandlePlayerCollision(Collider2D collider)
  {
    var healthBehaviour = collider.FindComponentInParentsOrThrow<PlayerHealthBehaviour>();

    var damageResult = healthBehaviour.ApplyDamage(DamageUnits, EnemyContactReaction.Knockback);

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

  public void DeactivateOnceInvisible()
  {
    Logger.UnityDebugLog("DeactivateOnceInvisible");

    _deactivateOnceInvisible = true;
  }
}
