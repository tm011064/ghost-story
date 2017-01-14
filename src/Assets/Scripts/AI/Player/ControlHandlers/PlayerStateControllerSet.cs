using System.Collections.Generic;

public class PlayerStateControllerSet : AbstractPlayerStateControllerSet
{
  private readonly PlayerStateController[] _playerStateControllers;

  public PlayerStateControllerSet(PlayerStateController[] playerStateControllers)
  {
    _playerStateControllers = playerStateControllers;
  }

  protected override IEnumerable<IPlayerStateUpdatable> Controllers
  {
    get { return _playerStateControllers; }
  }

  public override void Dispose()
  {
    for (var i = 0; i < _playerStateControllers.Length; i++)
    {
      _playerStateControllers[i].Dispose();
    }
  }
}
