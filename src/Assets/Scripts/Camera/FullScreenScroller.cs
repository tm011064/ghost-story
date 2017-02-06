public partial class FullScreenScroller : CameraScroller
{
  protected override void OnCameraScrollCompleted()
  {
    CameraController.Reset();
    CameraController.OnCameraModifierEnter(CameraMovementSettings);
  }
}