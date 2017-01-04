using UnityEngine;

public abstract class AbstractWeaponBehaviour : MonoBehaviour
{
  protected PlayerController Player;

  protected InputStateManager InputStateManager;

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

  public abstract PlayerStateUpdateResult UpdateState(XYAxisState axisState);

  public abstract string Name { get; }
}
