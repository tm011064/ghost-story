using System;
using UnityEngine;
using UnityEngine.UI;

public class BlackBarCanvas : MonoBehaviour
{
  private Image _image;

  private float? _fadeOutStartTime;

  private float? _fadeInStartTime;

  private float _fadeOutDuration;

  private float _fadeInDuration;

  public event Action FadeOutCompleted;

  public event Action FadeInCompleted;

  public void StartFadeIn(float duration)
  {
    _fadeInStartTime = Time.time;
    _fadeInDuration = duration;
  }

  public void StartFadeOut(float duration)
  {
    _fadeOutStartTime = Time.time;
    _fadeOutDuration = duration;
  }

  void Awake()
  {
    _image = GetComponentInChildren<Image>();
    _image.color = new Color(
      _image.color.r,
      _image.color.g,
      _image.color.b,
      1);
  }

  void Update()
  {
    if (_fadeOutStartTime.HasValue)
    {
      var progressPercentage = Mathf.Min(
        1,
        GameManager.Instance.Easing.GetValue(
          EasingType.Linear,
          Time.time - _fadeOutStartTime.Value,
          _fadeOutDuration));

      _image.color = new Color(
        _image.color.r,
        _image.color.g,
        _image.color.b,
        progressPercentage);

      if (progressPercentage == 1)
      {
        _fadeOutStartTime = null;

        if (FadeOutCompleted != null)
        {
          FadeOutCompleted();
        }
      }
    }
    // TODO (Roman): refactor
    if (_fadeInStartTime.HasValue)
    {
      var progressPercentage = Mathf.Min(
        1,
        GameManager.Instance.Easing.GetValue(
          EasingType.Linear,
          Time.time - _fadeInStartTime.Value,
          _fadeInDuration));

      _image.color = new Color(
        _image.color.r,
        _image.color.g,
        _image.color.b,
        1f - progressPercentage);

      if (progressPercentage == 1)
      {
        _fadeInStartTime = null;

        if (FadeInCompleted != null)
        {
          FadeInCompleted();
        }
      }
    }
  }
}
