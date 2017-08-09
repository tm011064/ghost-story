using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseCharacterController, IPlayerCollidable, ISpawnable
{
  [HideInInspector]
  public Animator Animator;

  private bool _deactivateOnceInvisible;

  private bool _isVisible;

  public event EventHandler<GameObjectEventArgs> GotDisabled;

  public virtual void Awake()
  {
    Animator = GetComponent<Animator>();
  }

  protected virtual void OnEnable()
  {
    _deactivateOnceInvisible = false;
  }

  protected virtual void OnDisable()
  {
    var handler = GotDisabled;
    if (handler != null)
    {
      handler(this, new GameObjectEventArgs(gameObject));
    }
  }

  public void AdjustVerticalSpriteScale(Direction direction)
  {
    if ((direction == Direction.Down && transform.localScale.y < 1)
      || (direction == Direction.Up && transform.localScale.y > -1))
    {
      transform.localScale = transform.localScale.MultiplyY(-1);
    }
  }

  public void AdjustHorizontalSpriteScale(Direction direction)
  {
    if ((direction == Direction.Right && transform.localScale.x < 1)
      || (direction == Direction.Left && transform.localScale.x > -1))
    {
      transform.localScale = transform.localScale.MultiplyX(-1);
    }
  }

  public void FlipSpriteHorizontally()
  {
    transform.localScale = transform.localScale.SetX(transform.localScale.x * -1);
  }

  public virtual void Reset(IDictionary<string, string> options)
  {
  }

  public virtual void OnPlayerCollide(PlayerController playerController)
  {
  }

  public virtual bool CanSpawn()
  {
    return true;
  }

  public virtual void OnBecameVisible()
  {
    _isVisible = true;
  }

  public virtual void OnBecameInvisible()
  {
    _isVisible = false;

    if (_deactivateOnceInvisible)
    {
      ObjectPoolingManager.Instance.Deactivate(gameObject);
    }
  }

  public void DeactivateOnceInvisible()
  {
    _deactivateOnceInvisible = true;

    if (!_isVisible)
    {
      ObjectPoolingManager.Instance.Deactivate(gameObject);
    }
  }
}
