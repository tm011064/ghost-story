#if UNITY_EDITOR

public partial class Checkpoint : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    transform.position = arguments.TiledRectBounds.center;
  }
}

#endif