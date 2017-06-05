using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemyController : BaseCharacterController, IPlayerCollidable, ISpawnable
{
  [HideInInspector]
  public Animator Animator;

  public event EventHandler<GameObjectEventArgs> GotDisabled;

  void Awake()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();
    if (CharacterPhysicsManager != null)
    {
      CharacterPhysicsManager.BoxCollider = GetComponent<BoxCollider2D>();
    }

    Animator = GetComponent<Animator>();

    OnAwake();
  }

  protected virtual void OnDisable()
  {
    var handler = GotDisabled;
    if (handler != null)
    {
      handler(this, new GameObjectEventArgs(gameObject));
    }
  }

  public void FlipSpriteHorizontally()
  {
    transform.localScale = transform.localScale.SetX(transform.localScale.x * -1);
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

  protected virtual void OnAwake()
  {
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
}
