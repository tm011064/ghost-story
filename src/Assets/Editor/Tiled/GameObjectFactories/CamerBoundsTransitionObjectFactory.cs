using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class CamerBoundsTransitionObjectFactory : AbstractGameObjectFactory
  {
    private readonly string _transitionObjectPrefabName;

    public CamerBoundsTransitionObjectFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName,
      string transitionObjectPrefabName)
      : base(map, prefabLookup, objecttypesByName)
    {
      _transitionObjectPrefabName = transitionObjectPrefabName;
    }

    public override IEnumerable<GameObject> Create(Property[] propertyFilters)
    {
      var cameraBounds = GetCameraBounds(propertyFilters).ToArray();

      var filters = propertyFilters
        .Concat(new Property[] { new Property { Name = "TransitionObjectPrefab", Value = _transitionObjectPrefabName } })
        .ToArray();

      var tag = GetTag(propertyFilters);

      return Map
        .ForEachLayerWithProperties(filters)
        .SelectMany(layer => CreateTransitionObjects(layer, tag, cameraBounds));
    }

    private string GetTag(Property[] propertyFilters)
    {
      var tagProperty = propertyFilters.FirstOrDefault(p => p.Name == "Tag");
      return tagProperty == null
        ? null
        : tagProperty.Value;
    }

    private IEnumerable<Bounds> GetCameraBounds(Property[] propertyFilters)
    {
      var cameraBoundsFilter = propertyFilters
        .Concat(new Property[] { new Property { Name = "Type", Value = "CameraBounds" } })
        .ToArray();

      return Map
        .ForEachObjectGroupWithProperties(cameraBoundsFilter)
        .SelectMany(og => og.Object.Select(o => o.GetBounds()));
    }

    private IEnumerable<GameObject> CreateTransitionObjects(Layer layer, string tag, IEnumerable<Bounds> cameraBounds)
    {
      var vertices = CreateMatrixVertices(layer);
      var transitionObjectBoundaries = vertices.GetRectangleBounds();

      var asset = LoadPrefabAsset(_transitionObjectPrefabName);

      foreach (var transitionObjectBounds in transitionObjectBoundaries)
      {
        var intersectingCameraBounds = cameraBounds
          .Where(bounds => bounds.Intersects(transitionObjectBounds))
          .ToArray();

        yield return CreateInstantiableObject(
          asset,
          _transitionObjectPrefabName,
          new CameraTransitionInstantiationArguments
          {
            TransitionObjectBounds = transitionObjectBounds,
            IntersectingCameraBounds = intersectingCameraBounds,
            Tag = tag
          });
      }
    }
  }
}
