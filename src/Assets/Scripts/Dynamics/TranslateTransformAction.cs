using System;
using UnityEngine;

public class TranslateTransformAction
{
  private readonly Vector3 _targetPosition;

  private readonly EasingType _easingType;

  private readonly float _duration;

  private float? _startTime;

  private float? _endTime;

  private Vector3 _path;

  private Vector3 _startPosition;

  public static TranslateTransformAction Start(
    Vector3 startPosition,
    Vector3 targetPosition,
    float duration,
    EasingType easingType)
  {
    var translateTransformAction = new TranslateTransformAction(
      targetPosition,
      duration,
      easingType);

    translateTransformAction.Start(startPosition);

    return translateTransformAction;
  }

  public TranslateTransformAction(
    Vector3 targetPosition,
    float duration,
    EasingType easingType)
  {
    _easingType = easingType;
    _targetPosition = targetPosition;
    _duration = duration;
  }

  public float Duration { get { return _duration; } }

  public bool IsStarted()
  {
    return _startTime.HasValue;
  }

  public bool IsCompleted()
  {
    return _endTime.HasValue && Time.time >= _endTime.Value;
  }

  public void Start(Vector3 startPosition)
  {
    _startTime = Time.time;
    _endTime = _startTime + _duration;

    _startPosition = startPosition;

    _path = _targetPosition - _startPosition;
  }

  public Vector3 GetPosition()
  {
    if (!_startTime.HasValue)
    {
      throw new InvalidOperationException("Action has not started yet");
    }

    if (Time.time >= _endTime.Value)
    {
      return _targetPosition;
    }

    float currentTime = Time.time - _startTime.Value;

    var percentage = Easing.GetValue(_easingType, currentTime, _duration);

    var translationVector = _startPosition + (_path.normalized * (_path.magnitude * percentage));

    return translationVector;
  }
}
