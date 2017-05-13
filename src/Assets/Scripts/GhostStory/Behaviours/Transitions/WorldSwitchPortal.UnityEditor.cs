#if UNITY_EDITOR

public partial class WorldSwitchPortal : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    transform.position = arguments.TiledRectBounds.center;

    TransitionToPortalName = arguments.TryProperty("Transition To Portal");
    TransitionToScene = arguments.TryProperty("Transition To Scene");
    PortalName = arguments.TiledObjectName;
  }
}

#endif