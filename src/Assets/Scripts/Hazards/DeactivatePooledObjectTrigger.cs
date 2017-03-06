using UnityEngine;

public partial class DeactivatePooledObjectTrigger : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D col)
  {
    var enemyHealthBehaviour = col.gameObject.GetComponent<EnemyHealthBehaviour>();
    if (enemyHealthBehaviour != null)
    {
      enemyHealthBehaviour.ApplyDamage(enemyHealthBehaviour.HealthUnits);
      return;
    }

    ObjectPoolingManager.Instance.Deactivate(col.gameObject);
  }
}
