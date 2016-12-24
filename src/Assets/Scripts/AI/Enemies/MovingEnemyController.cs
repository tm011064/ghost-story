﻿using UnityEngine;

public class MovingEnemyController : BaseCharacterController, IPlayerCollidable, ISpawnable
{
  void Awake()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();

    OnAwake();
  }

  protected virtual void OnAwake()
  {
  }

  public virtual void Reset()
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
