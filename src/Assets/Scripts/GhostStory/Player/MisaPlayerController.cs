public class MisaPlayerController : PlayerController
{
  protected override PlayerControlHandler CreateDefaultPlayerControlHandler()
  {
    return new UniverseSwitchPlayerControlHandler(this);
  }
}
