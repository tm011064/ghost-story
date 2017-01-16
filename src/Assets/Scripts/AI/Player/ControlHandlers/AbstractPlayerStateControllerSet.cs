using System.Collections.Generic;
using System.Linq;

public abstract class AbstractPlayerStateControllerSet
{
  protected abstract IEnumerable<IPlayerStateUpdatable> Controllers { get; }

  public virtual void Dispose()
  {
  }

  public PlayerStateUpdateResult Update(XYAxisState axisState)
  {
    foreach (var controller in Controllers)
    {
      var playerStateUpdateResult = controller.UpdatePlayerState(axisState);

      if (playerStateUpdateResult.IsHandled)
      {
        return playerStateUpdateResult;
      }
    }

    return PlayerStateUpdateResult.Unhandled;
  }
}