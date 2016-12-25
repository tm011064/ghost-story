using System.Collections.Generic;
using UnityEngine;

public class MovingEnemyController : BaseCharacterController, IPlayerCollidable, ISpawnable
{
  [HideInInspector]
  public Animator Animator;

  void Awake()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();
    CharacterPhysicsManager.BoxCollider = GetComponent<BoxCollider2D>();

    Animator = GetComponent<Animator>();

    OnAwake();
  }

  public void FlipSpriteHorizontally()
  {
    transform.localScale = transform.localScale.SetX(transform.localScale.x * -1);
  }

  public void AdjustSpriteScale(Direction direction)
  {
    if ((direction == Direction.Right && transform.localScale.x < 1f)
      || (direction == Direction.Left && transform.localScale.x > -1f))
    {
      transform.localScale = transform.localScale.SetX(
        transform.localScale.x * -1);
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
