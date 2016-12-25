using UnityEngine;

public class StaticEnemyControlHandler : BaseControlHandler
{
  private readonly float _duration;

  private readonly string _animationName;

  private readonly Animator _animator;

  private float _endTime;

  public StaticEnemyControlHandler(
    float duration,
    Animator animator = null,
    string animationName = null)
    : base(null)
  {
    _duration = duration;
    _animationName = animationName;
    _animator = animator;
  }

  public override bool TryActivate(BaseControlHandler previousControlHandler)
  {
    _endTime = Time.time + _duration;

    if (_animator != null && _animationName != null)
    {
      _animator.Play(_animationName);
    }

    return true;
  }

  protected override ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return Time.time >= _endTime
      ? ControlHandlerAfterUpdateStatus.CanBeDisposed
      : ControlHandlerAfterUpdateStatus.KeepAlive;
  }
}
