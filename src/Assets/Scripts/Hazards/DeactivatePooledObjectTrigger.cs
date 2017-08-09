using UnityEngine;

public partial class DeactivatePooledObjectTrigger : MonoBehaviour
{
  public PooledObjectType PooledObjectType = PooledObjectType.Default; // TODO (Roman): this must be set

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
