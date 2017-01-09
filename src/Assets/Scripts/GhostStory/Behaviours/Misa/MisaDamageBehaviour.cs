public class MisaDamageBehaviour : PlayerDamageBehaviour
{
  protected override void OnHealthChanged(int totalHealthUnits)
  {
    GameManager.Instance.Player.PushControlHandlers(
      new EnemyContactInvinciblePlayerControlHandler(
        GameManager.Instance.Player,
        4f),
      new EnemyContactKnockbackPlayerControlHandler(
        GameManager.Instance.Player,
        .3f,
        60f));
  }
}
