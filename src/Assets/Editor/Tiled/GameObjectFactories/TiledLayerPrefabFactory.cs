using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class TiledLayerPrefabFactory : AbstractGameObjectFactory
  {
    public TiledLayerPrefabFactory(
      GameObject root, 
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(root, map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      var prefabsParent = new GameObject("Prefab Group");
      // TODO (Roman): get commands and copy assign prefab mesh
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
              .ToDictionary()
          });
      }
    }
  }
}
