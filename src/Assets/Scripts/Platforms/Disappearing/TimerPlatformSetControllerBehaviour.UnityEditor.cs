#if UNITY_EDITOR

using System.Linq;

public partial class TimerPlatformSetControllerBehaviour : IInstantiable<TimerPlatformSetInstantiationArguments>
{
  public void Instantiate(TimerPlatformSetInstantiationArguments arguments)
  {
    ActivationDelay = arguments.ActivationDelay;
    VisibleInterval = arguments.VisibleInterval;
    InvisibleInterval = arguments.InvisibleInterval;

    var timerPlatformBuilder = new TimerPlatformBuilder(transform);
    timerPlatformBuilder.Build(arguments.Platforms).ToArray();
  }
}

#endif