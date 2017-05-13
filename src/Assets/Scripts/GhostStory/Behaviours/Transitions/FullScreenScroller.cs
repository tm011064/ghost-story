public partial class FullScreenScroller : CameraScroller
{
  protected override void OnAwake()
  {
    OverrideSettings(
      GhostStoryGameContext.Instance.GameSettings.SmoothDampMoveSettings,
      GhostStoryGameContext.Instance.GameSettings.CameraSettings,
      GhostStoryGameContext.Instance.GameSettings.HorizontalCamereaWindowSettings,
      GhostStoryGameContext.Instance.GameSettings.VerticalSnapWindowSettings);

    FullScreenScrollSettings = GhostStoryGameContext.Instance.GameSettings.FullScreenScrollSettings;
  }

  protected override void OnCameraScrollCompleted()
  {
    CameraController.Reset();
    CameraController.OnCameraModifierEnter(CameraMovementSettings);
  }
}