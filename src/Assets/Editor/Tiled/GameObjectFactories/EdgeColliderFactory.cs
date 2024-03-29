﻿using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class EdgeColliderFactory : AbstractGameObjectFactory
  {
    public EdgeColliderFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup)
      : base(root, map, prefabLookup)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return ObjectLayerConfigs
        .Where(config => config.Type == "EdgeColliders")
        .Select(config => CreateColliders(config));
    }

    private GameObject CreateColliders(TiledObjectLayerConfig layerConfig)
    {
      var collidersGameObject = new GameObject("Edge Colliders");

      foreach (var obj in layerConfig.TiledObjectGroup.Objects)
      {
        var edgeColliderObject = new GameObject("Edge Collider");

        edgeColliderObject.transform.position = new Vector2(obj.X, -obj.Y);

        edgeColliderObject.layer = LayerMask.NameToLayer("OneWayPlatform");

        var edgeCollider = edgeColliderObject.AddComponent<EdgeCollider2D>();

        edgeCollider.points = obj.PolyLine.ToVectors().ToArray();
        edgeColliderObject.AddComponent<OneWayPlatform>();

        edgeColliderObject.transform.parent = collidersGameObject.transform;
      }

      OnGameObjectCreated(layerConfig, collidersGameObject);

      return collidersGameObject;
    }
  }
}
