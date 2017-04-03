#if UNITY_EDITOR

public partial class NurseryTrapdoor : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    transform.position = arguments.TiledRectBounds.center.AddY(arguments.TiledRectBounds.size.y / 2);
  }
}

#endif
