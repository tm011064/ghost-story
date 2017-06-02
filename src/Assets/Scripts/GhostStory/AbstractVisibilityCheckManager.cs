using System;

public abstract class AbstractVisibilityCheckManager : IDisposable
{
  private readonly Universe _universe;

  private readonly float _interval;

  private readonly Func<bool> _checkVisibility;

  private readonly string _timerName;

  private bool _isVisible;

  private Action _gotVisibleCallback;

  private Action _gotHiddenCallback;

  protected AbstractVisibilityCheckManager(
    Universe universe,
    float interval,
    Func<bool> checkVisibility,
    Action gotVisibleCallback,
    Action gotHiddenCallback)
  {
    _universe = universe;
    _interval = interval;
    _checkVisibility = checkVisibility;
    _timerName = "Visibility Checks" + Guid.NewGuid().ToString();
    _gotVisibleCallback = gotVisibleCallback;
    _gotHiddenCallback = gotHiddenCallback;
  }

  public void Dispose()
  {
    StopChecks();

    _gotVisibleCallback = null;
    _gotHiddenCallback = null;
  }

  public void StartChecks()
  {
    GhostStoryGameContext.Instance.RegisterCallback(_interval, () => CheckVisibility(), _universe, _timerName);
  }

  public void StopChecks()
  {
    GhostStoryGameContext.Instance.CancelCallback(_universe, _timerName);
  }

  void CheckVisibility()
  {
    var isVisible = _checkVisibility();

    if (isVisible && !_isVisible)
    {
      if (_gotVisibleCallback != null)
      {
        _gotVisibleCallback();
      }
    }
    else if (!isVisible && _isVisible)
    {
      if (_gotHiddenCallback != null)
      {
        _gotHiddenCallback();
      }
    }

    _isVisible = isVisible;

    StartChecks();
  }
}
