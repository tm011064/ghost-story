public struct CharacterCollisionState2D
{
  public bool Right;

  public bool Left;

  public bool Above;

  public bool Below;

  public bool BecameGroundedThisFrame;

  public bool FacingDownSlope;

  public bool FacingUpSlope;

  public bool MovingUpSlope;

  public bool IsOnSlope;

  public float SlopeAngle;

  public CharacterWallState CharacterWallState;

  public bool IsFullyGrounded; // indicates whether the player is standing on an edge

  public bool WasGroundedLastFrame;

  public float LastTimeGrounded;

  public bool HasCollision()
  {
    return Below || Right || Left || Above;
  }

  public void Reset()
  {
    CharacterWallState = CharacterWallState.NotOnWall;

    Right = Left = Above = Below = BecameGroundedThisFrame = FacingDownSlope = IsFullyGrounded = MovingUpSlope = IsOnSlope = false;

    SlopeAngle = 0f;
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }
}