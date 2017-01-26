using System;

/// <summary>
/// check http://easings.net/
/// </summary>
public static class Easing
{
  private const float PI_HALF = (float)((float)Math.PI / 2d);

  public static float GetValue(EasingType easingType, float currentTime, float duration)
  {
    switch (easingType)
    {
      case EasingType.EaseInQuad: return EaseInQuad(currentTime, duration);

      case EasingType.EaseOutQuad: return EaseOutQuad(currentTime, duration);

      case EasingType.EaseInOutQuad: return EaseInOutQuad(currentTime, duration);

      case EasingType.EaseInCubic: return EaseInCubic(currentTime, duration);

      case EasingType.EaseOutCubic: return EaseOutCubic(currentTime, duration);

      case EasingType.EaseInOutCubic: return EaseInOutCubic(currentTime, duration);

      case EasingType.EaseInQuart: return EaseInQuart(currentTime, duration);

      case EasingType.EaseOutQuart: return EaseOutQuart(currentTime, duration);

      case EasingType.EaseInOutQuart: return EaseInOutQuart(currentTime, duration);

      case EasingType.EaseInQuint: return EaseInQuint(currentTime, duration);

      case EasingType.EaseOutQuint: return EaseOutQuint(currentTime, duration);

      case EasingType.EaseInOutQuint: return EaseInOutQuint(currentTime, duration);

      case EasingType.EaseInSine: return EaseInSine(currentTime, duration);

      case EasingType.EaseOutSine: return EaseOutSine(currentTime, duration);

      case EasingType.EaseInOutSine: return EaseInOutSine(currentTime, duration);

      case EasingType.EaseInExpo: return EaseInExpo(currentTime, duration);

      case EasingType.EaseOutExpo: return EaseOutExpo(currentTime, duration);

      case EasingType.EaseInOutExpo: return EaseInOutExpo(currentTime, duration);

      case EasingType.EaseInCirc: return EaseInCirc(currentTime, duration);

      case EasingType.EaseOutCirc: return EaseOutCirc(currentTime, duration);

      case EasingType.EaseInOutCirc: return EaseInOutCirc(currentTime, duration);

      case EasingType.EaseInBack: return EaseInBack(currentTime, duration);

      case EasingType.EaseOutBack: return EaseOutBack(currentTime, duration);

      case EasingType.EaseInOutBack: return EaseInOutBack(currentTime, duration);

      case EasingType.EaseInBounce: return EaseInBounce(currentTime, duration);

      case EasingType.EaseOutBounce: return EaseOutBounce(currentTime, duration);

      case EasingType.EaseInOutBounce: return EaseInOutBounce(currentTime, duration);

      case EasingType.Linear:
      default:
        return Linear(currentTime, duration);
    }
  }

  public static float Linear(float currentTime, float duration)
  {
    return currentTime / duration;
  }

  public static float EaseInQuad(float currentTime, float duration)
  {
    return 1f * (currentTime /= duration) * currentTime;
  }

  public static float EaseOutQuad(float currentTime, float duration)
  {
    return -1f * (currentTime /= duration) * (currentTime - 2);
  }

  public static float EaseInOutQuad(float currentTime, float duration)
  {
    if ((currentTime /= duration / 2) < 1) return 1f / 2 * currentTime * currentTime;

    return -1f / 2 * ((--currentTime) * (currentTime - 2) - 1);
  }

  public static float EaseInCubic(float currentTime, float duration)
  {
    return 1f * (currentTime /= duration) * currentTime * currentTime;
  }

  public static float EaseOutCubic(float currentTime, float duration)
  {
    return 1f * ((currentTime = currentTime / duration - 1) * currentTime * currentTime + 1);
  }

  public static float EaseInOutCubic(float currentTime, float duration)
  {
    if ((currentTime /= duration / 2) < 1)
    {
      return 1f / 2 * currentTime * currentTime * currentTime;
    }

    return 1f / 2 * ((currentTime -= 2) * currentTime * currentTime + 2);
  }

  public static float EaseInQuart(float currentTime, float duration)
  {
    return 1f * (currentTime /= duration) * currentTime * currentTime * currentTime;
  }

  public static float EaseOutQuart(float currentTime, float duration)
  {
    return -1f * ((currentTime = currentTime / duration - 1) * currentTime * currentTime * currentTime - 1);
  }

  public static float EaseInOutQuart(float currentTime, float duration)
  {
    if ((currentTime /= duration / 2) < 1)
    {
      return 1f / 2 * currentTime * currentTime * currentTime * currentTime;
    }

    return -1f / 2 * ((currentTime -= 2) * currentTime * currentTime * currentTime - 2);
  }

  public static float EaseInQuint(float currentTime, float duration)
  {
    return 1f * (currentTime /= duration) * currentTime * currentTime * currentTime * currentTime;
  }

  public static float EaseOutQuint(float currentTime, float duration)
  {
    return 1f * ((currentTime = currentTime / duration - 1) * currentTime * currentTime * currentTime * currentTime + 1);
  }

  public static float EaseInOutQuint(float currentTime, float duration)
  {
    if ((currentTime /= duration / 2) < 1)
    {
      return 1f / 2 * currentTime * currentTime * currentTime * currentTime * currentTime;
    }

    return 1f / 2 * ((currentTime -= 2) * currentTime * currentTime * currentTime * currentTime + 2);
  }

  public static float EaseInSine(float currentTime, float duration)
  {
    return -1f * (float)Math.Cos(currentTime / duration * (PI_HALF)) + 1f;
  }

  public static float EaseOutSine(float currentTime, float duration)
  {
    return 1f * (float)Math.Sin(currentTime / duration * (PI_HALF));
  }

  public static float EaseInOutSine(float currentTime, float duration)
  {
    return -1f / 2 * ((float)Math.Cos((float)Math.PI * currentTime / duration) - 1);
  }

  public static float EaseInExpo(float currentTime, float duration)
  {
    return (currentTime == 0) ? 0f : 1f * (float)Math.Pow(2, 10 * (currentTime / duration - 1));
  }

  public static float EaseOutExpo(float currentTime, float duration)
  {
    return (currentTime == duration) ? 1f : 1f * (-(float)Math.Pow(2, -10 * currentTime / duration) + 1);
  }

  public static float EaseInOutExpo(float currentTime, float duration)
  {
    if (currentTime == 0)
    {
      return 0f;
    }

    if (currentTime == duration)
    {
      return 1f;
    }

    if ((currentTime /= duration / 2) < 1)
    {
      return 1f / 2 * (float)Math.Pow(2, 10 * (currentTime - 1));
    }

    return 1f / 2 * (-(float)Math.Pow(2, -10 * --currentTime) + 2);
  }

  public static float EaseInCirc(float currentTime, float duration)
  {
    return -1f * ((float)Math.Sqrt(1 - (currentTime /= duration) * currentTime) - 1);
  }

  public static float EaseOutCirc(float currentTime, float duration)
  {
    return 1f * (float)Math.Sqrt(1 - (currentTime = currentTime / duration - 1) * currentTime);
  }

  public static float EaseInOutCirc(float currentTime, float duration)
  {
    if ((currentTime /= duration / 2) < 1)
    {
      return -1f / 2 * ((float)Math.Sqrt(1 - currentTime * currentTime) - 1);
    }

    return 1f / 2 * ((float)Math.Sqrt(1 - (currentTime -= 2) * currentTime) + 1);
  }

  public static float EaseInBack(float currentTime, float duration)
  {
    var s = 1.70158f;

    return 1f * (currentTime /= duration) * currentTime * ((s + 1) * currentTime - s);
  }

  public static float EaseOutBack(float currentTime, float duration)
  {
    var s = 1.70158f;

    return 1f * ((currentTime = currentTime / duration - 1) * currentTime * ((s + 1) * currentTime + s) + 1);
  }

  public static float EaseInOutBack(float currentTime, float duration)
  {
    var s = 1.70158f;

    if ((currentTime /= duration / 2) < 1)
    {
      return 1f / 2 * (currentTime * currentTime * (((s *= (1.525f)) + 1) * currentTime - s));
    }

    return 1f / 2 * ((currentTime -= 2) * currentTime * (((s *= (1.525f)) + 1) * currentTime + s) + 2);
  }

  public static float EaseInBounce(float currentTime, float duration)
  {
    return 1f - EaseOutBounce(duration - currentTime, duration);
  }

  public static float EaseOutBounce(float currentTime, float duration)
  {
    if ((currentTime /= duration) < (1 / 2.75))
    {
      return 1f * (7.5625f * currentTime * currentTime);
    }
    else if (currentTime < (2 / 2.75))
    {
      return 1f * (7.5625f * (currentTime -= (1.5f / 2.75f)) * currentTime + .75f);
    }
    else if (currentTime < (2.5 / 2.75))
    {
      return 1f * (7.5625f * (currentTime -= (2.25f / 2.75f)) * currentTime + .9375f);
    }
    else
    {
      return 1f * (7.5625f * (currentTime -= (2.625f / 2.75f)) * currentTime + .984375f);
    }
  }

  public static float EaseInOutBounce(float currentTime, float duration)
  {
    if (currentTime < duration / 2f)
    {
      return EaseInBounce(currentTime * 2f, duration) * .5f;
    }

    return EaseOutBounce(currentTime * 2f - duration, duration) * .5f + 1f * .5f;
  }
}
