public class GhostStoryCameraModifier : CameraModifier
{
  protected override void OnAwake()
  {
    // TODO (Roman): this can be unified/abstracted with CameraModifier which does the same thing
    OverrideSettings(
      GhostStoryGameContext.Instance.GameSettings.SmoothDampMoveSettings,
      GhostStoryGameContext.Instance.GameSettings.CameraSettings,
      GhostStoryGameContext.Instance.GameSettings.HorizontalCamereaWindowSettings,
      GhostStoryGameContext.Instance.GameSettings.VerticalSnapWindowSettings);
  }
}
