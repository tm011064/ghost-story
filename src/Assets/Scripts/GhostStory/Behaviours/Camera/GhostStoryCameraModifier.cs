public class GhostStoryCameraModifier : CameraModifier
{
  protected override void OnAwake()
  {
    OverrideSettings(
      GhostStoryGameContext.Instance.GameSettings.SmoothDampMoveSettings,
      GhostStoryGameContext.Instance.GameSettings.CameraSettings,
      GhostStoryGameContext.Instance.GameSettings.HorizontalCamereaWindowSettings,
      GhostStoryGameContext.Instance.GameSettings.VerticalSnapWindowSettings);
  }
}
