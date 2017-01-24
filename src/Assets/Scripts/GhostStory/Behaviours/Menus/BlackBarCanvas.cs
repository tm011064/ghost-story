using System;
using UnityEngine;
using UnityEngine.UI;

public class BlackBarCanvas : MonoBehaviour
{
  public EasingType Easing = EasingType.Linear;

  private Image _image;

  private float? _fadeStartTime;

  private float _fadeDuration;

  private Func<float, float> _alphaCalculator;

  public bool IsFading()
  {
    return _fadeStartTime.HasValue;
  }

  private void StartFade(float duration, Func<float, float> alphaCalculator)
  {
    _fadeStartTime = Time.time;
    _fadeDuration = duration;
    _alphaCalculator = alphaCalculator;
  }

  public void StartFadeIn(float duration)
  {
    StartFade(duration, (float progressPercentage) => 1f - progressPercentage);
  }

  public void StartFadeOut(float duration)
  {
    StartFade(duration, (float progressPercentage) => progressPercentage);
  }

  void Awake()
  {
    _image = GetComponentInChildren<Image>();
    _image.color = new Color(
      _image.color.r,
      _image.color.g,
      _image.color.b,
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
      var progressPercentage = Mathf.Min(
        1,
        GameManager.Instance.Easing.GetValue(Easing, Time.time - _fadeStartTime.Value, _fadeDuration));

      _image.color = new Color(
        _image.color.r,
        _image.color.g,
        _image.color.b,
        _alphaCalculator(progressPercentage));

      if (progressPercentage == 1)
      {
        _fadeStartTime = null;
      }
    }
  }
}
