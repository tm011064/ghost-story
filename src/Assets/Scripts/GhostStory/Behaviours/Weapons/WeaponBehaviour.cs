using UnityEngine;

public abstract class AbstractWeaponBehaviour : MonoBehaviour
{
  public abstract void StopAttack();

  public abstract bool IsAttacking();

  public abstract PlayerStateUpdateResult UpdateState(XYAxisState axisState);

  public abstract string Name { get; }
}
