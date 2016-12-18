#if UNITY_EDITOR

using System;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class Door : IInstantiable<CameraTransitionInstantiationArguments>
  {
    public void Instantiate(CameraTransitionInstantiationArguments arguments)
    {
      var center = arguments.TransitionObjectBounds.center;

      foreach (var intersectingCameraBounds in arguments.IntersectingCameraBounds)
      {
        var fullScreenScrollerGameObject = new GameObject("Full Screen Scroller");

        fullScreenScrollerGameObject.transform.position = center;
        fullScreenScrollerGameObject.layer = LayerMask.NameToLayer("PlayerTriggerMask");
        fullScreenScrollerGameObject.transform.parent = gameObject.transform;

        fullScreenScrollerGameObject
          .AddComponent<FullScreenScroller>()
          .Instantiate(new CameraModifierInstantiationArguments
            {
              Bounds = intersectingCameraBounds,
              BoundsPropertyInfos = new CameraModifierInstantiationArguments.BoundsPropertyInfo[] 
                {
                  new CameraModifierInstantiationArguments.BoundsPropertyInfo
                  {
                    Bounds = GetCameraModifierBounds(arguments, intersectingCameraBounds)
                  }
                }
            });
      }
    }

    private Bounds GetCameraModifierBounds(CameraTransitionInstantiationArguments arguments, Bounds cameraBounds)
    {
      var center = arguments.TransitionObjectBounds.center;

      if (CameraBoundsAreToTheRight(arguments.TransitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x + CameraModifierPadding.x / 2, center.y, center.z);
        var cameraModifierSize = new Vector2(Size.x - CameraModifierPadding.x, Size.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      if (CameraBoundsAreToTheLeft(arguments.TransitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x - CameraModifierPadding.x / 2, center.y, center.z);
        var cameraModifierSize = new Vector2(Size.x - CameraModifierPadding.x, Size.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      if (CameraBoundsAreAbove(arguments.TransitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x, center.y + CameraModifierPadding.y / 2, center.z);
        var cameraModifierSize = new Vector2(Size.x, Size.y - CameraModifierPadding.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      if (CameraBoundsAreBelow(arguments.TransitionObjectBounds, cameraBounds))
      {
        var cameraModifierCenter = new Vector3(center.x, center.y - CameraModifierPadding.y / 2, center.z);
        var cameraModifierSize = new Vector2(Size.x, Size.y - CameraModifierPadding.y);

        return new Bounds(cameraModifierCenter, cameraModifierSize);
      }

      throw new InvalidOperationException();
    }

    private bool CameraBoundsAreToTheRight(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainLeftEdgeOf(cameraBounds);
    }

    private bool CameraBoundsAreToTheLeft(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainRightEdgeOf(cameraBounds);
    }

    private bool CameraBoundsAreAbove(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainBottomEdgeOf(cameraBounds);
    }

    private bool CameraBoundsAreBelow(Bounds transitionObjectBounds, Bounds cameraBounds)
    {
      return transitionObjectBounds.ContainTopEdgeOf(cameraBounds);
    }
  }
}

#endif