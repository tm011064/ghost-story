using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class PlatformColliderFactory : AbstractGameObjectFactory
  {
    public PlatformColliderFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(root, map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return TileLayerConfigs
        .Where(c => c.Type == "Platform")
        .Select(config => CreateColliders(config));
    }

    private GameObject CreateColliders(TiledTileLayerConfig layerConfig)
    {
      var vertices = CreateMatrixVertices(layerConfig.TiledLayer);

      var name = layerConfig.Universe + " " + layerConfig.Layer + " Platform Colliders";

      var collidersGameObject = new GameObject(name);
      collidersGameObject.transform.position = Vector3.zero;

      var colliders = vertices.GetColliderEdges();

      foreach (var points in colliders)
      {
        var obj = new GameObject("Collider");

        obj.transform.position = Vector3.zero;
        obj.layer = LayerMask.NameToLayer("Platforms");

        AddEdgeColliders(obj, points);

        obj.transform.parent = collidersGameObject.transform;
      }

      OnGameObjectCreated(layerConfig, collidersGameObject);

      return collidersGameObject;
    }
  }
}
