#if UNITY_EDITOR

using System.Linq;

public partial class ConnectedTimerPlatformControllerBehaviour : IInstantiable<ConnectedTimerPlatformInstantiationArguments>
{
  public void Instantiate(ConnectedTimerPlatformInstantiationArguments arguments)
  {
    var timerPlatformBuilder = new TimerPlatformBuilder(transform);
    timerPlatformBuilder.Build(arguments.Platforms).ToArray();
  }
}

#endif