using System;

[Serializable]
public class EnemyMovementSettings
{
  public Direction StartDirection = Direction.Right;

  public float Speed;

  public float AirborneSpeed;

  public float Gravity;

  public EnemyMovementSettings Clone(
    Direction? startDirection = null,
    float? speed = null,
    float? airborneSpeed = null,
    float? gravity = null)
  {
    return new EnemyMovementSettings
    {
      StartDirection = startDirection ?? StartDirection,
      Speed = speed ?? Speed,
      Gravity = gravity ?? Gravity,
      AirborneSpeed = airborneSpeed ?? AirborneSpeed
    };
  }
}
