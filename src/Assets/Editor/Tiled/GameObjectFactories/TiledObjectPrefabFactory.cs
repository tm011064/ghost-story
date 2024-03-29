﻿using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class TiledObjectPrefabFactory : AbstractGameObjectFactory
  {
    public TiledObjectPrefabFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup)
      : base(root, map, prefabLookup)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      var prefabsParent = new GameObject("Prefab Group");

      prefabsParent.transform.position = Vector3.zero;

      var createdGameObjects = ObjectLayerConfigs
        .Where(config => config.Type == "PrefabGroup")
        .SelectMany(config => CreatePrefabFromGameObject(config));

      foreach (var gameObject in createdGameObjects)
      {
        gameObject.transform.parent = prefabsParent.transform;
      }

      yield return prefabsParent;
    }

    protected virtual IEnumerable<GameObject> CreatePrefabFromGameObject(TiledObjectLayerConfig layerConfig)
    {
      foreach (var obj in layerConfig.TiledObjectGroup.Objects)
      {
        var properties = obj.GetProperties();

        var prefabName = properties["Prefab"];

        var asset = LoadPrefabAsset(prefabName);

        yield return CreateInstantiableObject(
         asset,
         prefabName,
         layerConfig,
         new InstantiationArguments
         {
           Bounds = obj.GetBounds(),
           Properties = properties,
           IsFlippedHorizontally = IsFlippedHorizontally(obj),
           IsFlippedVertically = IsFlippedVertically(obj)
         },
         obj.Name);
      }
    }
  }
}
