#if UNITY_EDITOR

public partial class WorldSwitchPortal : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    arguments.CheckHasTiledObjectName();

    transform.position = arguments.TiledRectBounds.center;

    PortalName = arguments.TiledObjectName;
  }
}

#endif