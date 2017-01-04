public partial class FullScreenScroller : CameraScroller
{
  protected override void OnCameraScrollCompleted()
  {
    CameraController.ClearCameraModifiers();
    CameraController.OnCameraModifierEnter(CameraMovementSettings);
  }
}