using UnityEngine;

public abstract class AbstractWeaponBehaviour : MonoBehaviour
{
  protected PlayerController Player;

  protected InputStateManager InputStateManager;

  public Sprite HudIcon;

  void Awake()
  {
    InputStateManager = GameManager.Instance.InputStateManager;
    Player = GetComponentInParent<PlayerController>();

    OnAwake();
  }

  protected virtual void OnAwake()
  {
  }

  public abstract void StopAttack();

  public abstract bool IsAttacking();

  public abstract PlayerStateUpdateResult UpdatePlayerState(XYAxisState axisState);

  public virtual void Dispose()
  {
  }

  public abstract string Name { get; }
}
