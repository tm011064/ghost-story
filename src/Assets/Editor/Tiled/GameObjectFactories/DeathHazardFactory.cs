using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class DeathHazardFactory : AbstractGameObjectFactory
  {
    public DeathHazardFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, ObjectType> objectTypesByName)
      : base(root, map, prefabLookup, objectTypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return TileLayerConfigs
        .Where(config => config.Type == "DeathHazard")
        .Select(config => CreateColliders(config));
    }

    private GameObject CreateColliders(TiledTileLayerConfig layerConfig)
    {
      var vertices = CreateMatrixVertices(layerConfig.TiledLayer);

      var collidersGameObject = new GameObject("Death Hazard Colliders");
      collidersGameObject.transform.position = Vector3.zero;

      int padding;
      layerConfig.TiledLayer.TryGetProperty("Collider Padding", out padding);

      var colliders = vertices.GetColliderEdges(padding);

      foreach (var points in colliders)
      {
        var obj = new GameObject("Death Hazard Trigger Collider");

        obj.transform.position = Vector3.zero;
        obj.layer = LayerMask.NameToLayer("PlayerTriggerMask");

        AddEdgeColliders(obj, points, padding, true);

        obj.AddComponent<InstantDeathHazardTrigger>();

        obj.transform.parent = collidersGameObject.transform;
      }

      OnGameObjectCreated(layerConfig, collidersGameObject);

      return collidersGameObject;
    }
  }
}
