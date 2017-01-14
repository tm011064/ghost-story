public class KinoDamageBehaviour : PlayerDamageBehaviour
{
  protected override void OnHealthChanged(int totalHealthUnits, EnemyContactReaction enemyContactReaction)
  {
    Logger.UnityDebugLog("KINO DAMAGE, HEALTH: " + totalHealthUnits);
  }
}
