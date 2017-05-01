#if UNITY_EDITOR

using System.Linq;

public partial class AbstractEnemySpawnManager : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    SpawnArguments = arguments
      .Properties
      .Select(kvp => new SpawnArgument { Key = kvp.Key, Value = kvp.Value })
      .ToArray();

    transform.position = arguments.TiledRectBounds.center;

    transform.ForEachChildComponent<IInstantiable<PrefabInstantiationArguments>>(
      instantiable => instantiable.Instantiate(arguments));
  }
}

#endif