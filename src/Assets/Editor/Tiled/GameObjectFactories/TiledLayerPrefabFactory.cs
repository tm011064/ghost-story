using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class TiledLayerPrefabFactory : AbstractGameObjectFactory
  {
    public TiledLayerPrefabFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, ObjectType> objectTypesByName)
      : base(root, map, prefabLookup, objectTypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      var prefabsParent = new GameObject("Prefab Group");

      prefabsParent.transform.position = Vector3.zero;

      var createdGameObjects = TileLayerConfigs
        .Where(config => config.Type == "PrefabGroup")
        .SelectMany(config => CreatePrefabsFromLayer(config));

      foreach (var gameObject in createdGameObjects)
      {
        gameObject.transform.parent = prefabsParent.transform;
      }

      yield return prefabsParent;
    }

    private IEnumerable<GameObject> CreatePrefabsFromLayer(TiledTileLayerConfig layerConfig)
    {
      var prefabName = layerConfig.TiledLayer.PropertyGroup
        .Properties
        .First(p => string.Equals(p.Name.Trim(), "Prefab", StringComparison.OrdinalIgnoreCase))
        .Value
        .Trim()
        .ToLower();

      var asset = LoadPrefabAsset(prefabName);

      var matrixVertices = CreateMatrixVertices(layerConfig.TiledLayer);

      foreach (var bounds in matrixVertices.GetRectangleBounds())
      {
        yield return CreateInstantiableObject(
          asset,
          prefabName,
          layerConfig,
          new InstantiationArguments
          {
            Bounds = bounds,
            Properties = layerConfig.TiledLayer
              .PropertyGroup
              .ToDictionary()
          });
      }
    }
  }
}
