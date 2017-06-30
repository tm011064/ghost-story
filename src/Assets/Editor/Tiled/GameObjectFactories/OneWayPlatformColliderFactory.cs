using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class OneWayPlatformColliderFactory : AbstractGameObjectFactory
  {
    public OneWayPlatformColliderFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup)
      : base(root, map, prefabLookup)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return TileLayerConfigs
        .Where(config => config.Type == "OneWayPlatform")
        .Select(config => CreateColliders(config));
    }

    private GameObject CreateColliders(TiledTileLayerConfig layerConfig)
    {
      var vertices = CreateMatrixVertices(layerConfig.TiledLayer);

      var collidersGameObject = new GameObject("One Way Platform Colliders");
      collidersGameObject.transform.position = Vector3.zero;

      var edgePoints = vertices.GetTopColliderEdges();

      foreach (var edge in edgePoints)
      {
        var edgeColliderObject = new GameObject("Edge Collider");

        var extents = new Vector2(
          edge.To.x - edge.From.x,
          edge.To.y - edge.From.y) * .5f;

        edgeColliderObject.transform.position = new Vector3(
          edge.To.x - extents.x,
          edge.To.y - extents.y);

        edgeColliderObject.layer = LayerMask.NameToLayer("OneWayPlatform");

        var edgeCollider = edgeColliderObject.AddComponent<EdgeCollider2D>();

        edgeCollider.points = new Vector2[] 
        { 
          new Vector2(-extents.x, extents.y),
          new Vector2(extents.x, extents.y)
        };

        edgeColliderObject.AddComponent<OneWayPlatform>();

        edgeColliderObject.transform.parent = collidersGameObject.transform;
      }

      OnGameObjectCreated(layerConfig, collidersGameObject);

      return collidersGameObject;
    }
  }
}
