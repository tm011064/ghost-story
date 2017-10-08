using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerPlatformSetFactory : AbstractGameObjectFactory
{
  private const string PrefabName = "Timer Platform Set Controller";

  public TimerPlatformSetFactory(
    GameObject root,
    Map map,
    Dictionary<string, string> prefabLookup)
    : base(root, map, prefabLookup)
  {
  }

  public override IEnumerable<GameObject> Create()
  {
    var prefabsParent = new GameObject("Timer Platform Sets");

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
    var trimmedLayerName = layer.Name.Trim();

    int value;
    if (!int.TryParse(trimmedLayerName, out value))
    {
      throw new FormatException("Timed platform timer platform layer name '" + trimmedLayerName
        + "' invalid. Names must be a number representing the index of the platform within the platform group");
    }

    return value;
  }

  protected virtual GameObject CreatePrefabFromGameObject(TiledGroupConfig config)
  {
    var platforms = config
      .TileLayerConfigs
      .Select(c => c.TiledLayer)
      .Where(c => c.HasProperty("Type", "Timer Platform"))
      .Select(layer => new PlatformArguments
      {
        Index = ParseIndex(layer),
        ColliderObjects = CreateColliderObjects(layer).ToArray(),
        Transform = Root.gameObject.transform.FindFirstRecursive(layer.Name)
      })
      .ToArray();

    var asset = LoadPrefabAsset(PrefabName);

    return CreateInstantiableObject(
       asset,
       PrefabName,
       config,
       new TimerPlatformSetInstantiationArguments
       {
         ActivationDelay = config.Group.GetPropertyValueAsFloat("ActivationDelay"),
         InvisibleInterval = config.Group.GetPropertyValueAsFloat("InvisibleInterval"),
         VisibleInterval = config.Group.GetPropertyValueAsFloat("VisibleInterval"),
         Platforms = platforms
       });
  }
}
