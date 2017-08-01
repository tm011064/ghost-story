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

    public Transform Transform;

    public GameObject[] ColliderObjects;
  }
}
