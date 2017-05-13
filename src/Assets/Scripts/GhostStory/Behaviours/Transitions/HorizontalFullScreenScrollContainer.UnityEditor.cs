#if UNITY_EDITOR

using System;
using UnityEngine;

public partial class HorizontalFullScreenScrollContainer : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    var center = arguments.TiledRectBounds.center;
    transform.position = center;

    foreach (var intersectingCameraBounds in arguments.IntersectingCameraTriggerBounds)
    {
      var fullScreenScroller = new GameObject("Scroller");

      fullScreenScroller.layer = LayerMask.NameToLayer("PlayerTriggerMask");
      fullScreenScroller.transform.parent = gameObject.transform;

      var triggerLocation = GetTriggerLocation(
        arguments.TiledRectBounds,
        intersectingCameraBounds.TriggerBounds); // TODO (Roman): this needs to be trigger bounds, not camera bounds

      var triggerBounds = GetInnerCameraModifierBounds(
        arguments.TiledRectBounds,
        triggerLocation,
        CameraModifierPadding);

      fullScreenScroller
        .AddComponent<FullScreenScroller>()
        .Instantiate(new FullScreenScrollerInstantiationArguments
          {
            CameraBounds = intersectingCameraBounds.CameraBounds,
            TriggerBounds = triggerBounds,
            TriggerLocation = triggerLocation
          });
    }
  }

  private Direction GetTriggerLocation(
    Bounds transitionObjectBounds,
    Bounds cameraBounds)
  {
    if (transitionObjectBounds.ContainLeftEdgeOf(cameraBounds))
    {
      return Direction.Right;
    }

    if (transitionObjectBounds.ContainRightEdgeOf(cameraBounds))
    {
      return Direction.Left;
    }

    if (transitionObjectBounds.ContainBottomEdgeOf(cameraBounds))
    {
      return Direction.Down;
    }

    if (transitionObjectBounds.ContainTopEdgeOf(cameraBounds))
    {
      return Direction.Up;
    }

    throw new InvalidOperationException();
  }

  protected Bounds GetInnerCameraModifierBounds(
    Bounds transitionObjectBounds,
    Direction location,
    Vector2 padding)
  {
    var center = transitionObjectBounds.center;

    Vector3 cameraModifierCenter;
    Vector2 cameraModifierSize;

    switch (location)
    {
      case Direction.Left:
        cameraModifierCenter = center.SetX(transitionObjectBounds.max.x - padding.x / 2);
        cameraModifierSize = new Vector2(padding.x, transitionObjectBounds.size.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);

      case Direction.Right:
        cameraModifierCenter = center.SetX(transitionObjectBounds.min.x + padding.x / 2);
        cameraModifierSize = new Vector2(padding.x, transitionObjectBounds.size.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);

      case Direction.Up:
        cameraModifierCenter = center.SetY(transitionObjectBounds.max.y - padding.y / 2);
        cameraModifierSize = new Vector2(transitionObjectBounds.size.x, padding.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);

      case Direction.Down:
        cameraModifierCenter = center.SetY(transitionObjectBounds.min.y + padding.y / 2);
        cameraModifierSize = new Vector2(transitionObjectBounds.size.x, padding.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
    }

    throw new InvalidOperationException();
  }
}

#endif