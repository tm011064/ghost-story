public class MisaPlayerController : GhostStoryPlayerController
{
  private WorldSwitchSettings _worldSwitchSettings;

  protected override void OnAwake()
  {
    _worldSwitchSettings = GetComponent<WorldSwitchSettings>();

    base.OnAwake();
  }

  protected override PlayerControlHandler CreateDefaultPlayerControlHandler()
  {
    return new UniverseSwitchPlayerControlHandler(this, _worldSwitchSettings);
  }
}
