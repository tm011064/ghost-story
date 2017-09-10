#if UNITY_EDITOR

using System.Linq;

public partial class JumpControlledDisappearingPlatformControllerBehaviour : IInstantiable<JumpControlledDisappearingPlatformInstantiationArguments>
{
  public void Instantiate(JumpControlledDisappearingPlatformInstantiationArguments arguments)
  {
    MaxVisiblePlatforms = arguments.MaxVisiblePlatforms;

    var timerPlatformBuilder = new TimerPlatformBuilder(transform);
    timerPlatformBuilder.Build(arguments.Platforms).ToArray();
  }
}

#endif