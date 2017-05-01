using System;
using System.Collections;
using UnityEngine;

public class BaseMonoBehaviour : MonoBehaviour
{
  public event Action<BaseMonoBehaviour> GotDisabled;

  public event Action<BaseMonoBehaviour> GotEnabled;

  public event Action<BaseMonoBehaviour> GotVisible;

  public event Action<BaseMonoBehaviour> GotHidden;

  protected CameraController CameraController;

  protected bool IsVisible;

  private float _visibiltyCheckInterval = 0f;

  protected virtual void OnGotVisible()
  {
  }

  protected virtual void OnGotHidden()
  {
  }

  protected virtual void OnEnable()
  {
    CameraController = Camera.main.GetComponent<CameraController>();

    var handler = GotEnabled;
    if (handler != null)
    {
      handler(this);
    }
  }

  protected virtual void OnDisable()
  {
    IsVisible = false;

    var handler = GotDisabled;
    if (handler != null)
    {
      handler(this);
    }
  }

  private void StartVisibilityChecks(float visibiltyCheckInterval, Func<bool> isVisible)
  {
    if (visibiltyCheckInterval > 0f)
    {
      _visibiltyCheckInterval = visibiltyCheckInterval;

      StartCoroutine(CheckVisibility(isVisible));
    }
  }

  protected void StopVisibilityChecks()
  {
    StopCoroutine("CheckVisibility");
  }

  protected void StartVisibilityChecks(float visibiltyCheckInterval, Collider2D collider)
  {
    StartVisibilityChecks(
      visibiltyCheckInterval,
      () => CameraController
        .CalculateScreenBounds()
        .Intersects(collider.bounds));
  }

  protected void StartVisibilityChecks(float visibiltyCheckInterval, Renderer renderer)
  {
    StartVisibilityChecks(visibiltyCheckInterval, () => renderer.IsVisibleFrom(Camera.main));
  }

  IEnumerator CheckVisibility(Func<bool> testVisibility)
  {
    while (true)
    {
      var isVisible = testVisibility();

      if (isVisible && !IsVisible)
      {
        OnGotVisible();

        var handler = GotVisible;

        if (handler != null)
        {
          handler(this);
        }
      }
      else if (!isVisible && IsVisible)
      {
        OnGotHidden();

        var handler = GotHidden;

        if (handler != null)
        {
          handler(this);
        }
      }

      IsVisible = isVisible;

      yield return new WaitForSeconds(_visibiltyCheckInterval);
    }
  }

  /// <summary>
  /// Unity seems to set the bounds extents of a disabled game object's box collider to zero. This method
  /// returns the actual extents of the collider as if it was enabled.
  /// </summary>
  protected Bounds GetBounds(BoxCollider2D collider)
  {
    if (isActiveAndEnabled)
    {
      return collider.bounds;
    }

    var bounds = collider.bounds;

    bounds.size = collider.size;

    return bounds;
  }
}
