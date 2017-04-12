#if UNITY_EDITOR

using System;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class OpenOnlyDoor : IInstantiable<PrefabInstantiationArguments>
  {
    public void Instantiate(PrefabInstantiationArguments arguments)
    {
      var center = arguments.TiledRectBounds.center;
      transform.position = center;

      var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();
      boxCollider.size = arguments.TiledRectBounds.size;

      foreach (var intersectingCameraBounds in arguments.IntersectingCameraBounds)
      {
        var doorLocation = GetDoorLocation(
          arguments.TiledRectBounds,
          intersectingCameraBounds);

        var triggerBounds = GetOuterCameraModifierBounds(
          arguments.TiledRectBounds,
          doorLocation,
          CameraModifierPadding);

        var boxColliderGameObject = CreateBoxColliderGameObject(triggerBounds, "Door Handle Trigger");

        var triggerEnterBehaviour = boxColliderGameObject.AddComponent<DoorTriggerEnterBehaviour>();
        triggerEnterBehaviour.DoorKeysNeededToEnter = new DoorKey[] { DoorKey };
        triggerEnterBehaviour.DoorLocation = doorLocation;
      }
    }

    private GameObject CreateBoxColliderGameObject(
      Bounds bounds,
      string name = "Box Collider With Enter Trigger") // TODO (Roman): copy/paste -> abstract
    {
      var boxColliderGameObject = new GameObject(name);

      boxColliderGameObject.transform.position = bounds.center;
      boxColliderGameObject.layer = gameObject.layer;
      boxColliderGameObject.transform.parent = gameObject.transform;

      var boxCollider = boxColliderGameObject.AddComponent<BoxCollider2D>();

      boxCollider.isTrigger = true;
      boxCollider.size = bounds.size;

      return boxColliderGameObject;
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

    protected Bounds GetInnerCameraModifierBounds(
      Bounds transitionObjectBounds,
      Bounds cameraBounds,
      Vector2 padding)
    {
      var center = transitionObjectBounds.center;

      if (CameraBoundsAreToTheRight(transitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x + padding.x / 2, center.y, center.z);
        var cameraModifierSize = new Vector2(transitionObjectBounds.size.x - padding.x, transitionObjectBounds.size.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      if (CameraBoundsAreToTheLeft(transitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x - padding.x / 2, center.y, center.z);
        var cameraModifierSize = new Vector2(transitionObjectBounds.size.x - padding.x, transitionObjectBounds.size.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      if (CameraBoundsAreAbove(transitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x, center.y + padding.y / 2, center.z);
        var cameraModifierSize = new Vector2(transitionObjectBounds.size.x, transitionObjectBounds.size.y - padding.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      if (CameraBoundsAreBelow(transitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x, center.y - padding.y / 2, center.z);
        var cameraModifierSize = new Vector2(transitionObjectBounds.size.x, transitionObjectBounds.size.y - padding.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      throw new InvalidOperationException();
    }

    protected bool CameraBoundsAreToTheRight(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainLeftEdgeOf(cameraBounds);
    }

    protected bool CameraBoundsAreToTheLeft(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainRightEdgeOf(cameraBounds);
    }

    protected bool CameraBoundsAreAbove(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainBottomEdgeOf(cameraBounds);
    }

    protected bool CameraBoundsAreBelow(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainTopEdgeOf(cameraBounds);
    }
  }
}

#endif