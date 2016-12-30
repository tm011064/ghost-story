using UnityEngine;

public class Yoyo : AbstractYoyo, IWeapon
{
  private BaseControlHandler _controlHandler;

  public override string Name { get { return "Yoyo"; } }

  protected override void OnStartAttack()
  {
    _controlHandler = new FreezePlayerControlHandler(
      GameManager.Player,
      10f,
      Animator.StringToHash(AttackAnimation));

    GameManager.Player.PushControlHandler(_controlHandler);
  }

  protected override void OnStopAttack()
  {
    GameManager.Player.RemoveControlHandler(_controlHandler);
    _controlHandler = null;
  }

  protected override string GetAttackAnimation(XYAxisState axisState)
  {
    if (GameManager.Player.IsAirborne()
        && axisState.IsDown())
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
