using UnityEngine;

public class MovingEnemyController : EnemyController, IPlayerCollidable, ISpawnable
{
  public override void Awake()
  {
    base.Awake();

    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();
    if (CharacterPhysicsManager != null)
    {
      CharacterPhysicsManager.BoxCollider = GetComponent<BoxCollider2D>();
    }
  }
}
