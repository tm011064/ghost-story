public class MisaPlayerController : PlayerController
{
  private WorldSwitchSettings _worldSwitchSettings;

  protected override void OnAwake()
  {
    _worldSwitchSettings = GetComponent<WorldSwitchSettings>();
  }

  protected override PlayerControlHandler CreateDefaultPlayerControlHandler()
  {
    return new UniverseSwitchPlayerControlHandler(this, _worldSwitchSettings);
  }
}
