using UnityEngine;

public class MovingEnemyController : EnemyController, IPlayerCollidable, ISpawnable
{
  protected override void OnAwake()
  {
    base.OnAwake();

    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();
    if (CharacterPhysicsManager != null)
    {
      CharacterPhysicsManager.BoxCollider = GetComponent<BoxCollider2D>();
    }
  }
}
