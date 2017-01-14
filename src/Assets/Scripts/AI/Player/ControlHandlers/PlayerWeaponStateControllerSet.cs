using System.Collections.Generic;
using System.Linq;

public class PlayerWeaponStateControllerSet : AbstractPlayerStateControllerSet
{
  private readonly PlayerController _playerController;

  public PlayerWeaponStateControllerSet(PlayerController playerController)
  {
    _playerController = playerController;
  }

  protected override IEnumerable<IPlayerStateUpdatable> Controllers
  {
    get
    {
      if ((_playerController.PlayerState & PlayerState.Locked) != 0)
      {
        return Enumerable.Empty<IPlayerStateUpdatable>();
      }

      return _playerController
        .Weapons
        .Where(w => w.isActiveAndEnabled)
        .Cast<IPlayerStateUpdatable>();
    }
  }
}
