public interface IWeapon : IPlayerStateUpdatable
{
  void StopAttack();

  bool IsAttacking();

  string Name { get; }
}