using System;
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
    Dictionary<string, string> prefabLookup)
    : base(root, map, prefabLookup)
  {
  }

  public override IEnumerable<GameObject> Create()
  {
    var prefabsParent = new GameObject("Timer Platforms");

    prefabsParent.transform.position = Vector3.zero;

    var createdGameObjects = GroupConfigs
      .Where(config => config.Type == PrefabName)
      .Select(config => CreatePrefabFromGameObject(config));

    foreach (var gameObject in createdGameObjects)
    {
      gameObject.transform.parent = prefabsParent.transform;
    }

    yield return prefabsParent;
  }

  private int ParseIndex(Layer layer)
  {
    var createErrorMessage = new Func<string>(() => "Timed platform timer platform layer name '" + layer.Name
        + "' invalid. Names must follow this pattern: 'unqiueName {Index}'. Example: 'Roof Platforms 1'");

    var index = layer.Name.LastIndexOf(' ');
    if (index < 0)
    {
      throw new FormatException(createErrorMessage());
    }

    var indexString = layer.Name.Substring(index).Trim();
    int value;
    if (!int.TryParse(indexString, out value))
    {
      throw new FormatException(createErrorMessage());
    }

    return value;
  }

  protected virtual GameObject CreatePrefabFromGameObject(TiledGroupConfig config)
  {
    var platforms = config
      .TileLayerConfigs
      .Select(c => c.TiledLayer)
      .Where(c => c.HasProperty("Type", "Timer Platform"))
      .Select(layer => new TimerPlatformInstantiationArguments.Platform
      {
        Index = ParseIndex(layer),
        ColliderObjects = CreateColliderObjects(layer).ToArray(),
        Transform = Root.gameObject.transform.FindFirstRecursive(layer.Name)
      })
      .ToArray();

    var startSwitch = config
      .ObjectLayerConfigs
      .SelectMany(c => c.TiledObjectGroup.Objects)
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
