using UnityEngine;

public abstract class PlayerDeathBehaviour : MonoBehaviour
{
  private PlayerHealthBehaviour _playerHealth;

  protected abstract void OnPlayerDied();

  void Awake()
  {
    _playerHealth = this.GetComponentOrThrow<PlayerHealthBehaviour>();

    _playerHealth.PlayerDied += OnPlayerDied;
  }

  void OnDestroy()
  {
    _playerHealth.PlayerDied -= OnPlayerDied;
  }
}
