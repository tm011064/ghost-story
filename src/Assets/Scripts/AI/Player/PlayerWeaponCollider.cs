using UnityEngine;

public class PlayerWeaponCollider : MonoBehaviour
{
  public int DamageUnits = 1;

  void OnTriggerEnter2D(Collider2D collider)
  {
    Debug.Log("HIT");
    HandleEnemyCollision(collider);
  }

  private void HandleEnemyCollision(Collider2D collider)
  {
    var enemyHealthBehaviour = collider.GetComponentOrThrow<EnemyHealthBehaviour>();

    enemyHealthBehaviour.ApplyDamage(DamageUnits);
  }
}
