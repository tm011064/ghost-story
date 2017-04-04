using UnityEngine;

public class Yoyo : AbstractYoyo, IWeapon
{
  private BaseControlHandler _controlHandler;

  public override string Name { get { return "Yoyo"; } }

  protected override void OnStartAttack()
  {
    _controlHandler = new FreezePlayerControlHandler(
      Player,
      -1,
      Animator.StringToHash(AttackAnimation),
      new PlayerState[] { PlayerState.Locked });

    Player.PushControlHandler(_controlHandler);
  }

  protected override void OnStopAttack()
  {
    Player.RemoveControlHandler(_controlHandler);
    _controlHandler = null;
  }

  protected override string GetAttackAnimation(XYAxisState axisState)
  {
    if (axisState.IsDown())
    {
      return "Yoyo Down";
    }

    if (axisState.IsUp())
    {
      return "Yoyo Up";
    }

    return "Yoyo";
  }
}
