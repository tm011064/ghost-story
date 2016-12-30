public interface IWeapon
{
  void StopAttack();

  bool IsAttacking();

  PlayerStateUpdateResult UpdateState(XYAxisState axisState);

  string Name { get; }
}