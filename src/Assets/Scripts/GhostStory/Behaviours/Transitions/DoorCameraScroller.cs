using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class DoorCameraScroller : CameraScroller
  {
    protected override void OnCameraScrollCompleted()
    {
      CameraController.ClearCameraModifiers();
      CameraController.OnCameraModifierEnter(CameraMovementSettings);
    }

    public void TriggerScroll(Collider2D collider)
    {
      OnEnter(collider);
    }
  }
}
