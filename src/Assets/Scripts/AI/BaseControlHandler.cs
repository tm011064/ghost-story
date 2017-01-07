using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Control handler's define how a character reacts to user input in respect to the character's
/// current state and environment.
/// </summary>
public class BaseControlHandler : IDisposable
{
  protected GameManager GameManager;

  private float? _timeRemaining;

  protected float Duration;

  protected CharacterPhysicsManager CharacterPhysicsManager;

  protected Color DebugBoundingBoxColor = Color.green;

  protected bool DoDrawDebugBoundingBox = false;

  public BaseControlHandler(CharacterPhysicsManager characterPhysicsManager)
    : this(characterPhysicsManager, -1)
  {
  }

  public BaseControlHandler(CharacterPhysicsManager characterPhysicsManager, float duration)
  {
    GameManager = GameManager.Instance;
    CharacterPhysicsManager = characterPhysicsManager;

    ResetOverrideEndTime(duration);
  }

  protected virtual void OnAfterUpdate()
  {
  }

  public virtual void OnAfterStackPeekUpdate()
  {
  }

  public virtual void Dispose()
  {
  }

  public virtual void OnFreeze()
  {
  }

  public virtual void OnUnfreeze(float freezeDuration)
  {
  }

  public virtual BaseControlHandler Clone()
  {
    return MemberwiseClone() as BaseControlHandler;
  }

  protected void ResetOverrideEndTime(float duration)
  {
    Duration = duration;
    _timeRemaining = duration >= 0f ? (float?)duration : null;
  }

  protected float? GetTimeRemaining()
  {
    return _timeRemaining;
  }

  protected virtual ControlHandlerAfterUpdateStatus DoUpdate()
  {
    return ControlHandlerAfterUpdateStatus.KeepAlive;
  }

  /// <summary>
  /// This method is called from the BaseCharacterController control handler stack in order to evaluate whether the
  /// top stack element can be activated or not. By default this method always returns true but can be overridden
  /// for special purposes or chained control handlers.
  /// </summary>
  /// <param name="previousControlHandler">The last active control handler.</param>
  /// <returns>true if activation was successful; false if not.</returns>
  public virtual bool TryActivate(BaseControlHandler previousControlHandler)
  {
    Logger.Trace("Activated control handler: " + ToString());

    return true;
  }

  public ControlHandlerAfterUpdateStatus Update()
  {
    if (_timeRemaining.HasValue)
    {
      _timeRemaining -= Time.deltaTime;

      if (_timeRemaining <= 0f)
      {
        return ControlHandlerAfterUpdateStatus.CanBeDisposed;
      }
    }

    var doUpdate = DoUpdate();

    OnAfterUpdate();

    return doUpdate;
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  protected void SetDebugDraw(
    Color debugBoundingBoxColor,
    bool doDrawDebugBoundingBox)
  {
    DebugBoundingBoxColor = debugBoundingBoxColor;
    DoDrawDebugBoundingBox = doDrawDebugBoundingBox;
  }

  [Conditional("DEBUG"), Conditional("PROFILE")]
  public virtual void DrawGizmos()
  {
  }
}
