using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BlackBarCanvas : MonoBehaviour
{
  public EasingType FadeInEasing = EasingType.EaseOutQuad;

  public EasingType FadeOutEasing = EasingType.EaseInQuad;

  private SpriteRenderer _spriteRenderer;

  private float? _fadeStartTime;

  private float _fadeDuration;

  private Func<float, float> _alphaCalculator;

  private Func<float> _percentageCalculator;

  private Action _onFadeCompleted;

  void Awake()
  {
    var cameraController = Camera.main.GetComponent<CameraController>();
    transform.parent = cameraController.gameObject.transform;

    _spriteRenderer = GetComponent<SpriteRenderer>();

    SetAlpha(0);
  }

  void OnDisable()
  {
    _fadeStartTime = null;
  }

  private float CalculateProgress()
  {
    var progressPercentage = Mathf.Min(1, _percentageCalculator());

    return progressPercentage > .9f
      ? 1
      : progressPercentage;
  }

  void Update()
  {
    if (_fadeStartTime.HasValue)
    {
      var progressPercentage = CalculateProgress();

      var alpha = _alphaCalculator(progressPercentage);

      SetAlpha(alpha);

      if (progressPercentage == 1)
      {
        StopFade();
      }
    }
  }

  private void StopFade()
  {
    _fadeStartTime = null;

    if (_onFadeCompleted != null)
    {
      _onFadeCompleted();
      _onFadeCompleted = null;
    }
  }

  public void ShowBlackScreen()
  {
    ResetTransform();

    SetAlpha(1);
  }

  public bool IsBlackScreen()
  {
    return _spriteRenderer.color.a == 1;
  }

  public void StartFadeIn(float duration, Action onFadeCompleted = null)
  {
    StartFade(
      duration,
      (float progressPercentage) => 1f - progressPercentage,
      () => Easing.GetValue(FadeInEasing, Time.time - _fadeStartTime.Value, _fadeDuration),
      onFadeCompleted);
  }

  public void StartFadeOut(float duration, Action onFadeCompleted = null)
  {
    StartFade(
      duration,
      (float progressPercentage) => progressPercentage,
      () => Easing.GetValue(FadeOutEasing, Time.time - _fadeStartTime.Value, _fadeDuration),
      onFadeCompleted);
  }

  public bool IsFading()
  {
    return _fadeStartTime.HasValue;
  }

  private void StartFade(
    float duration,
    Func<float, float> alphaCalculator,
    Func<float> percentageCalculator,
    Action onFadeCompleted = null)
  {
    Assert.IsFalse(_fadeStartTime.HasValue, "Fade already in progress");

    _onFadeCompleted = onFadeCompleted;

    ResetTransform();

    _fadeStartTime = Time.time;
    _fadeDuration = duration;
    _alphaCalculator = alphaCalculator;
    _percentageCalculator = percentageCalculator;
  }

  private void ResetTransform()
  {
    var cameraController = Camera.main.GetComponent<CameraController>();
    var screenSize = cameraController.GetScreenSize();

    transform.position = new Vector3(
      transform.position.x,
      transform.position.y,
      cameraController.ZAxisOffset);

    // TODO (old): this doesn't always work
    transform.localScale = new Vector3(
      screenSize.x * 1000,
      screenSize.y * 1000);
  }

  private void SetAlpha(float alpha)
  {
    _spriteRenderer.color = new Color(
      _spriteRenderer.color.r,
      _spriteRenderer.color.g,
      _spriteRenderer.color.b,
      alpha);
  }
}
