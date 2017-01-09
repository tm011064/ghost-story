using UnityEngine;

public abstract class PlayerDamageBehaviour : MonoBehaviour
{
  private PlayerHealthBehaviour _playerHealth;

  protected abstract void OnHealthChanged(int totalHealthUnits);

  void Awake()
  {
    _playerHealth = this.GetComponentOrThrow<PlayerHealthBehaviour>();

    _playerHealth.HealthChanged += OnHealthChanged;
  }

  void OnDestroy()
  {
    _playerHealth.HealthChanged -= OnHealthChanged;
  }
}
