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

  public bool IsFading()
  {
    return _fadeStartTime.HasValue;
  }

  private void StartFade(float duration, Func<float, float> alphaCalculator, Func<float> percentageCalculator)
  {
    var cameraController = Camera.main.GetComponent<CameraController>();
    var screenSize = cameraController.GetScreenSize();

    transform.position = new Vector3(
      transform.position.x,
      transform.position.y,
      cameraController.CameraOffset.z);

    transform.localScale = new Vector3(
      screenSize.x,
      screenSize.y);

    _fadeStartTime = Time.time;
    _fadeDuration = duration;
    _alphaCalculator = alphaCalculator;
    _percentageCalculator = percentageCalculator;
  }

  public void StartFadeIn(float duration, Action onFadeCompleted = null)
  {
    Assert.IsFalse(_fadeStartTime.HasValue, "Fade already in progress");

    _onFadeCompleted = onFadeCompleted;
    StartFade(
      duration,
      (float progressPercentage) => 1f - progressPercentage,
      () => Easing.GetValue(FadeInEasing, Time.time - _fadeStartTime.Value, _fadeDuration));
  }

  public void StartFadeOut(float duration, Action onFadeCompleted = null)
  {
    Assert.IsFalse(_fadeStartTime.HasValue, "Fade already in progress");

    _onFadeCompleted = onFadeCompleted;
    StartFade(
      duration,
      (float progressPercentage) => progressPercentage,
      () => Easing.GetValue(FadeOutEasing, Time.time - _fadeStartTime.Value, _fadeDuration));
  }

  void Awake()
  {
    var cameraController = Camera.main.GetComponent<CameraController>();
    transform.parent = cameraController.gameObject.transform;

    _spriteRenderer = GetComponent<SpriteRenderer>();

    _spriteRenderer.color = new Color(
      _spriteRenderer.color.r,
      _spriteRenderer.color.g,
      _spriteRenderer.color.b,
      0);
  }

  void OnDisable()
  {
    _fadeStartTime = null;
  }

  void Update()
  {
    if (_fadeStartTime.HasValue)
    {
      var progressPercentage = Mathf.Min(1, _percentageCalculator());
      if (progressPercentage >= .9f)
      {
        progressPercentage = 1;
      }

      _spriteRenderer.color = new Color(
        _spriteRenderer.color.r,
        _spriteRenderer.color.g,
        _spriteRenderer.color.b,
        _alphaCalculator(progressPercentage));

      if (progressPercentage == 1)
      {
        _fadeStartTime = null;

        if (_onFadeCompleted != null)
        {
          _onFadeCompleted();
          _onFadeCompleted = null;
        }
      }
    }
  }
}
