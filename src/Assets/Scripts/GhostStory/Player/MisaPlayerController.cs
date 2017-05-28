public class MisaPlayerController : GhostStoryPlayerController
{
  //private WorldSwitchSettings _worldSwitchSettings; // TODO (Roman): this will need to be refactored, world switch types are defined by game progress

  protected override void OnAwake()
  {
    //_worldSwitchSettings = GetComponent<WorldSwitchSettings>();

    AnimationHashLookup.Register(
      "Yoyo",
      "Yoyo Up",
      "Yoyo Down",
      "Yoyo 360",
      "Yoyo 180"); // TODO (Roman): kino player controller and register here

    base.OnAwake();
  }

  protected override PlayerControlHandler CreateDefaultPlayerControlHandler()
  {
    return new DefaultPlayerControlHandler(this);
  }
}
