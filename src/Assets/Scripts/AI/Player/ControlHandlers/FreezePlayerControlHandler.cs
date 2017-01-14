using UnityEngine;

public class FreezePlayerControlHandler : PlayerControlHandler
{
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
