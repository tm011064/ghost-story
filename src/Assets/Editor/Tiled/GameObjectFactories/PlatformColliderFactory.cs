using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class PlatformColliderFactory : AbstractGameObjectFactory
  {
    public PlatformColliderFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup)
      : base(root, map, prefabLookup)
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
      var name = layerConfig.Universe + " Platform Colliders";

      var collidersGameObject = new GameObject(name);
      collidersGameObject.transform.position = Vector3.zero;

      foreach (var obj in CreateColliderObjects(layerConfig.TiledLayer))
      {
        obj.transform.parent = collidersGameObject.transform;
      }

      OnGameObjectCreated(layerConfig, collidersGameObject);

      return collidersGameObject;
    }
  }
}
