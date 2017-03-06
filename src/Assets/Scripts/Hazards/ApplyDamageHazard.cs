using UnityEngine;

public partial class ApplyDamageHazard : MonoBehaviour
{
  public int PlayerDamageUnits = 1;

  public bool DestroyHazardOnCollision = false;

  public EnemyContactReaction EnemyContactReaction = EnemyContactReaction.Knockback;

  private void ApplyDamage()
  {
    if ((GameManager.Instance.Player.PlayerState & PlayerState.Invincible) != 0)
    {
      return;
    }

    if (DestroyHazardOnCollision)
    {
      ObjectPoolingManager.Instance.Deactivate(gameObject);
    }

    GameManager.Instance.Player.Health.ApplyDamage(PlayerDamageUnits, EnemyContactReaction);
  }

  void OnTriggerStay2D(Collider2D col)
  {
    ApplyDamage();
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    ApplyDamage();
  }
}
