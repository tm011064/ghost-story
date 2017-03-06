using System;

public class PlayerHealthBehaviour : BaseMonoBehaviour
{
  public int HealthUnits = 28;

  private int _currentHealthUnits;

  public event Action<int, EnemyContactReaction> HealthChanged;

  public event Action PlayerDied;

  public void Reset()
  {
    SetHealthUnits(HealthUnits);
  }

  protected void SetHealthUnits(
    int healthUnits,
    EnemyContactReaction enemyContactReaction = EnemyContactReaction.None)
  {
    Logger.UnityDebugLog("Health chanded", healthUnits, "c", enemyContactReaction);
    _currentHealthUnits = healthUnits;

    var handler = HealthChanged;
    if (handler != null)
    {
      Logger.UnityDebugLog("handle", _currentHealthUnits);
      handler(_currentHealthUnits, enemyContactReaction);
    }
  }

  private void NotifyPlayerDied()
  {
    var handler = PlayerDied;
    if (handler != null)
    {
      handler();
    }
  }

  public DamageResult DeductAllHealth()
  {
    if ((GameManager.Instance.Player.PlayerState & PlayerState.Invincible) != 0)
    {
      return DamageResult.Invincible;
    }

    SetHealthUnits(0);

    NotifyPlayerDied();

    return DamageResult.Destroyed;
  }

  public DamageResult ApplyDamage(int healthUnitsToDeduct, EnemyContactReaction enemyContactReaction)
  {
    if ((GameManager.Instance.Player.PlayerState & PlayerState.Invincible) != 0)
    {
      return DamageResult.Invincible;
    }

    SetHealthUnits(_currentHealthUnits - healthUnitsToDeduct, enemyContactReaction);

    if (_currentHealthUnits <= 0)
    {
      NotifyPlayerDied();

      return DamageResult.Destroyed;
    }

    return DamageResult.HealthReduced;
  }
}
