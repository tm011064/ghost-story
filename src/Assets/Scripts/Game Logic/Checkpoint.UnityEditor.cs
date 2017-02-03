#if UNITY_EDITOR

public partial class Checkpoint : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    transform.position = arguments.TiledRectBounds.center;

    if (arguments.Properties.GetBool("Is Level Start"))
    {
      Index = 0;
    }
    else
    {
      Index = 1;
    }
  }
}

#endif