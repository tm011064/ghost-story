using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class TiledObjectPrefabFactory : AbstractGameObjectFactory
  {
    public TiledObjectPrefabFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create(Property[] propertyFilters)
    {
      var prefabsParent = new GameObject("Auto created Tiled prefabs");
      prefabsParent.transform.position = Vector3.zero;

      var prefabGameObjects = GetObjects(propertyFilters)
        .Get<GameObject>(CreatePrefabFromGameObject);

      foreach (var gameObject in prefabGameObjects)
      {
        gameObject.transform.parent = prefabsParent.transform;
      }

      yield return prefabsParent;
    }

    private IEnumerable<Object> GetObjects(Property[] propertyFilters)
    {
      return propertyFilters.Any()
        ? Map.ForEachObjectWithProperty(propertyFilters, "Prefab", ObjecttypesByName)
        : Map.ForEachObjectWithProperty("Prefab", ObjecttypesByName);
    }

    private GameObject CreatePrefabFromGameObject(Object obj)
    {
      var properties = obj.GetProperties(ObjecttypesByName);

      var prefabName = properties["prefab"];

      var asset = LoadPrefabAsset(prefabName);

      return CreateInstantiableObject(
       asset,
       prefabName,
       new InstantiationArguments
       {
         Bounds = obj.GetBounds(),
         Properties = properties,
         IsFlippedHorizontally = obj.Gid >= 2000000000,
         IsFlippedVertically = (obj.Gid >= 1000000000 && obj.Gid < 2000000000) || obj.Gid >= 3000000000
       });
    }
  }
}
