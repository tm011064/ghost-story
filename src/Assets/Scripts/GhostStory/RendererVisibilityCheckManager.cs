using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RendererVisibilityCheckManager : AbstractVisibilityCheckManager
{
  public static RendererVisibilityCheckManager Create(
    Renderer renderer,
    Universe universe,
    Action gotVisibleCallback = null,
    Action gotHiddenCallback = null,
    float intervalInSeconds = .1f)
  {
    Assert.IsTrue(intervalInSeconds > 0);

    return new RendererVisibilityCheckManager(
      intervalInSeconds,
      universe,
      () => renderer.IsVisibleFrom(Camera.main),
      gotVisibleCallback,
      gotHiddenCallback);
  }

  private RendererVisibilityCheckManager(
    float interval,
    Universe universe,
    Func<bool> checkVisibility,
    Action gotVisibleCallback,
    Action gotHiddenCallback)
    : base(universe, interval, checkVisibility, gotVisibleCallback, gotHiddenCallback)
  {
  }
}

