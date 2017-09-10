using UnityEngine;

public class ConnectedTimerPlatformInstantiationArguments : AbstractInstantiationArguments
{
  public StartSwitch Switch;

  public PlatformArguments[] Platforms;

  public class StartSwitch
  {
    public Bounds Bounds;
  }
}
