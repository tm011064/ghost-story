public class KinoDamageBehaviour : PlayerDamageBehaviour
{
  protected override void OnHealthChanged(int totalHealthUnits)
  {
    Logger.UnityDebugLog("KINO DAMAGE, HEALTH: " + totalHealthUnits);
  }
}
