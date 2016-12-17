using UnityEngine;

public class CameraTransitionInstantiationArguments : AbstractInstantiationArguments
{
  public Bounds TransitionObjectBounds;

  public Bounds[] IntersectingCameraBounds;

  public string Tag;
}
