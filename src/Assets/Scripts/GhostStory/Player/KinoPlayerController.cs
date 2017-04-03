public class KinoPlayerController : GhostStoryPlayerController
{
  protected override void OnAwake()
  {
    AnimationHashLookup.Register(
      "Shoot",
      "Shoot Up",
      "Shoot Down");

    base.OnAwake();
  }
}
