using UnityEngine;

public class TimerPlatformInstantiationArguments : AbstractInstantiationArguments
{
  public StartSwitch Switch;

  public Platform[] Platforms;

  public class StartSwitch
  {
    public Bounds Bounds;
  }

  public class Platform
  {
    public int Index;

    public Bounds Bounds;

    public Transform Transform;
  }
}
