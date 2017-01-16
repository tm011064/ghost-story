using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class CamerBoundsTransitionObjectFactory : AbstractGameObjectFactory
  {
    public CamerBoundsTransitionObjectFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(root, map, prefabLookup, objecttypesByName)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      var cameraBoundsByLayerName = CreateCameraBoundsByLayerNameLookup();

      return TileLayerConfigs
        .Where(og => og.Type == "Transition")
        .SelectMany(layerConfig => CreateTransitionObjects(
          layerConfig,
          cameraBoundsByLayerName[layerConfig.Layer]));
    }

    private ILookup<string, Bounds> CreateCameraBoundsByLayerNameLookup()
    {
      return ObjectLayerConfigs
        .Where(config => config.Type == "CameraBounds")
        .Select(config =>
          new
          {
            Layer = config.Layer,
            Bounds = config.TiledObjectgroup.Object.Select(o => o.GetBounds())
          })
        .SelectMany(c => c.Bounds.Select(b => new { Layer = c.Layer, Bounds = b }))
        .ToLookup(c => c.Layer, c => c.Bounds);
    }

    private IEnumerable<GameObject> CreateTransitionObjects(
      TiledTileLayerConfig layerConfig,
      IEnumerable<Bounds> cameraBounds)
    {
      var vertices = CreateMatrixVertices(layerConfig.TiledLayer);
      var transitionObjectBoundaries = vertices.GetRectangleBounds();

      var prefabName = layerConfig.TiledLayer.GetPropertyValue("Prefab");

      var asset = LoadPrefabAsset(prefabName);

      foreach (var transitionObjectBounds in transitionObjectBoundaries)
      {
        var intersectingCameraBounds = cameraBounds
          .Where(bounds => bounds.Intersects(transitionObjectBounds))
          .ToArray();

        yield return CreateInstantiableObject(
          asset,
          prefabName,
          layerConfig,
          new CameraTransitionInstantiationArguments
          {
            TransitionObjectBounds = transitionObjectBounds,
            IntersectingCameraBounds = intersectingCameraBounds,
            PrefabsAssetPathsByShortName = PrefabLookup
          });
      }
    }
  }
}
