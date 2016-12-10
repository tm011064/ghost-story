using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class PlatformColliderFactory : AbstractGameObjectFactory
  {
    public PlatformColliderFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create(Property[] propertyFilters)
    {
      var filters = propertyFilters
        .Concat(new Property[] { new Property { Name = "Collider", Value = "platform" } })
        .ToArray();

      return Map
        .ForEachLayerWithProperties(filters)
        .Get<GameObject>(CreateColliders);
    }

    private GameObject CreateColliders(Layer layer)
    {
      var vertices = CreateMatrixVertices(layer);

      var collidersGameObject = new GameObject("Platform Colliders");
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

      return collidersGameObject;
    }
  }
}
