public class MisaDamageBehaviour : PlayerDamageBehaviour
{
  protected override void OnHealthChanged(int totalHealthUnits, EnemyContactReaction enemyContactReaction)
  {
    switch (enemyContactReaction)
    {
      case EnemyContactReaction.Knockback:
        GameManager.Instance.Player.PushControlHandlers(
          new MisaInvinciblePlayerControlHandler(
            GameManager.Instance.Player,
            1),
          new EnemyContactKnockbackPlayerControlHandler(
            GameManager.Instance.Player,
            .8f,
            20));
        break;
    }
  }
}
