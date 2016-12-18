using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class LayerPrefabFactory : AbstractGameObjectFactory
  {
    public LayerPrefabFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      var parentObject = new GameObject("Auto created Tiled layer objects");

      parentObject.transform.position = Vector3.zero;

      var createdGameObjects = TileLayerConfigs
        .Where(config => config.TiledLayer.HasProperty("Prefab"))
        .SelectMany(config => CreatePrefabsFromLayer(config));

      foreach (var gameObject in createdGameObjects)
      {
        gameObject.transform.parent = parentObject.transform;
      }

      yield return parentObject;
    }

    private IEnumerable<GameObject> CreatePrefabsFromLayer(TiledTileLayerConfig layerConfig)
    {
      var prefabName = layerConfig.TiledLayer.Properties
        .Property
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
              .Properties
              .Property
              .ToDictionary(p => p.Name, p => p.Value, StringComparer.InvariantCultureIgnoreCase)
          });
      }
    }
  }
}
