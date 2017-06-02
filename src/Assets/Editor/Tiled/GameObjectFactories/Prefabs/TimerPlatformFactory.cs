using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

public class TimerPlatformFactory : AbstractGameObjectFactory
{
  private const string PrefabName = "Timer Platform Controller";

  public TimerPlatformFactory(
    GameObject root,
    Map map,
    Dictionary<string, string> prefabLookup,
    Dictionary<string, ObjectType> objectTypesByName)
    : base(root, map, prefabLookup, objectTypesByName)
  {
  }

  public override IEnumerable<GameObject> Create()
  {
    var prefabsParent = new GameObject("Timer Platforms");

    prefabsParent.transform.position = Vector3.zero;

    var createdGameObjects = ObjectLayerConfigs
      .Where(config => config.Type == PrefabName)
      .Select(config => CreatePrefabFromGameObject(config));

    foreach (var gameObject in createdGameObjects)
    {
      gameObject.transform.parent = prefabsParent.transform;
    }

    yield return prefabsParent;
  }

  protected virtual GameObject CreatePrefabFromGameObject(TiledObjectLayerConfig config)
  {
    var objectGroupGameObject = Root.gameObject.transform.FindFirstRecursive(config.TiledObjectGroup.Name);

    var platforms = config
      .TiledObjectGroup
      .Objects
      .Where(o => o.IsType("Timer Platform"))
      .Select(o => new TimerPlatformInstantiationArguments.Platform
      {
        Index = int.Parse(o.Name),
        Bounds = o.GetBounds(),
        Transform = objectGroupGameObject.FindFirstRecursive(o.Name)
      })
      .ToArray();

    var startSwitch = config
      .TiledObjectGroup
      .Objects
      .Where(o => o.IsType("Switch"))
      .Select(o => new TimerPlatformInstantiationArguments.StartSwitch
      {
        Bounds = o.GetBounds()
      })
      .Single();

    var asset = LoadPrefabAsset(PrefabName);

    return CreateInstantiableObject(
       asset,
       PrefabName,
       config,
       new TimerPlatformInstantiationArguments
       {
         Switch = startSwitch,
         Platforms = platforms
       });
  }
}
