public class MisaPlayerController : GhostStoryPlayerController
{
  private WorldSwitchSettings _worldSwitchSettings;

  protected override void OnAwake()
  {
    _worldSwitchSettings = GetComponent<WorldSwitchSettings>();

    AnimationHashLookup.Register(
      "Yoyo",
      "Yoyo Up",
      "Yoyo Down",
      "Yoyo 360",
      "Yoyo 180");

    base.OnAwake();
  }

  protected override PlayerControlHandler CreateDefaultPlayerControlHandler()
  {
    return new UniverseSwitchPlayerControlHandler(this, _worldSwitchSettings);
  }
}
