using UnityEngine;

public struct MoveCalculationResult
{
  public CharacterCollisionState2D CollisionState;

  public Vector3 DeltaMovement;

  public Vector3 OriginalDeltaMovement;

  public HorizontalDirection HorizontalDirection;

  public HorizontalDirection PreviousHorizontalDirection;

  public CharacterCollisionState2D PreviousCollisionState;

  public Vector3 PreviousDeltaMovement;

  public bool HasHorizontalDirectionChanged()
  {
    return HorizontalDirection != PreviousHorizontalDirection;
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }
}
