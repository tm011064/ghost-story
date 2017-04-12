using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseCharacterController, IPlayerCollidable, ISpawnable
{
  [HideInInspector]
  public Animator Animator;

  void Awake()
  {
    Animator = GetComponent<Animator>();

    OnAwake();
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
