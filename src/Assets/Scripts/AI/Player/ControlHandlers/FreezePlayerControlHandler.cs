using UnityEngine;

public class FreezePlayerControlHandler : PlayerControlHandler
{
  public static FreezePlayerControlHandler CreateInvincible(
    string animationName,
    float suspendPhysicsTime = -1)
  {
    return new FreezePlayerControlHandler(
      GameManager.Instance.Player,
      suspendPhysicsTime,
      Animator.StringToHash(animationName),
      new PlayerState[] { PlayerState.Locked, PlayerState.Invincible });
  }

  private readonly PlayerState[] _playerStates;

  public FreezePlayerControlHandler(
    PlayerController playerController,
    float suspendPhysicsTime,
    int animationShortNameHash,
    PlayerState[] playerStates)
    : base(
      playerController,
      new PlayerStateController[] 
      { 
        new PlaySpecificAnimationPlayerStateController(playerController, animationShortNameHash)
      },
      suspendPhysicsTime)
  {
    SetDebugDraw(Color.red, true);

    _playerStates = playerStates;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    foreach (var playerState in _playerStates)
    {
      PlayerController.PlayerState |= playerState;
    }

    ResetOverrideEndTime(Duration);

    return true;
  }

  public override void Dispose()
  {
    foreach (var playerState in _playerStates)
    {
      PlayerController.PlayerState &= ~playerState;
    }
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
