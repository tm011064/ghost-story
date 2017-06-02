#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class Door : IInstantiable<PrefabInstantiationArguments>
  {
    public void Instantiate(PrefabInstantiationArguments arguments)
    {
      var center = arguments.TiledRectBounds.center;
      transform.position = center;

      var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();
      boxCollider.size = arguments.TiledRectBounds.size;

      foreach (var intersectingCameraBounds in arguments.IntersectingCameraBounds)
      {
        var assetPath = arguments.PrefabsAssetPathsByShortName["Door Scroller"];
        var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
        var scrollerGameObject = Instantiate(asset, Vector3.zero, Quaternion.identity) as GameObject;

        scrollerGameObject.layer = LayerMask.NameToLayer("PlayerTriggerMask");
        scrollerGameObject.transform.parent = gameObject.transform;

        var doorLocation = GetDoorLocation(
          arguments.TiledRectBounds,
          intersectingCameraBounds);

        var triggerBounds = GetOuterCameraModifierBounds(
          arguments.TiledRectBounds,
          doorLocation,
          CameraModifierPadding);

        scrollerGameObject
          .GetComponent<DoorCameraScroller>()
          .Instantiate(new DoorInstantiationArguments
          {
            CameraBounds = intersectingCameraBounds,
            TriggerBounds = triggerBounds,
            DoorKey = DoorKey,
            DoorLocation = doorLocation
          });
      }
    }

    private Direction GetDoorLocation(
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

    protected Bounds GetOuterCameraModifierBounds(
      Bounds transitionObjectBounds,
      Direction doorLocation,
      Vector2 padding)
    {
      var center = transitionObjectBounds.center;

      Vector3 cameraModifierCenter;
      Vector2 cameraModifierSize;

      switch (doorLocation)
      {
        case Direction.Left:
          cameraModifierCenter = center.SetX(transitionObjectBounds.max.x + padding.x / 2);
          cameraModifierSize = new Vector2(padding.x, transitionObjectBounds.size.y);

          return new Bounds(cameraModifierCenter, cameraModifierSize);

        case Direction.Right:
          cameraModifierCenter = center.SetX(transitionObjectBounds.min.x - padding.x / 2);
          cameraModifierSize = new Vector2(padding.x, transitionObjectBounds.size.y);

          return new Bounds(cameraModifierCenter, cameraModifierSize);

        case Direction.Up:
          cameraModifierCenter = center.SetY(transitionObjectBounds.max.y + padding.y / 2);
          cameraModifierSize = new Vector2(transitionObjectBounds.size.x, padding.y);

          return new Bounds(cameraModifierCenter, cameraModifierSize);

        case Direction.Down:
          cameraModifierCenter = center.SetY(transitionObjectBounds.min.y - padding.y / 2);
          cameraModifierSize = new Vector2(transitionObjectBounds.size.x, padding.y);

          return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      throw new InvalidOperationException();
    }
  }
}

#endif