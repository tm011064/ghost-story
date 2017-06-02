using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ColliderVisibilityCheckManager : AbstractVisibilityCheckManager
{
  public static ColliderVisibilityCheckManager Create(
    Collider2D collider,
    Universe universe,
    Action gotVisibleCallback = null,
    Action gotHiddenCallback = null,
    float intervalInSeconds = .1f)
  {
    Assert.IsTrue(intervalInSeconds > 0);

    var cameraController = Camera.main.GetComponent<CameraController>();

    return new ColliderVisibilityCheckManager(
      intervalInSeconds,
      universe,
      () => cameraController.CalculateScreenBounds().Intersects(collider.bounds),
      gotVisibleCallback,
      gotHiddenCallback);
  }

  private ColliderVisibilityCheckManager(
    float interval,
    Universe universe,
    Func<bool> checkVisibility,
    Action gotVisibleCallback,
    Action gotHiddenCallback)
    : base(universe, interval, checkVisibility, gotVisibleCallback, gotHiddenCallback)
  {
  }
}