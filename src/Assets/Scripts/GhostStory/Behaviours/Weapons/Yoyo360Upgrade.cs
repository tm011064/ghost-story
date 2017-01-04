using UnityEngine;

public class Yoyo360Upgrade : AbstractYoyo, IWeapon
{
  private BaseControlHandler _controlHandler;

  public override string Name { get { return "Yoyo 360"; } }

  protected override void OnStartAttack()
  {
    if (Player.IsGrounded())
    {
      _controlHandler = new FreezePlayerControlHandler(
        Player,
        10f,
        Animator.StringToHash(AttackAnimation));

      Player.PushControlHandler(_controlHandler);
    }
  }

  protected override void OnStopAttack()
  {
    if (_controlHandler != null)
    {
      Player.RemoveControlHandler(_controlHandler);
      _controlHandler = null;
    }
  }

  protected override string GetAttackAnimation(XYAxisState axisState)
  {
    return "Yoyo 360";
  }
}
