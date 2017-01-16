using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class CamerBoundsLayerTransitionObjectFactory : AbstractGameObjectFactory
  {
    public CamerBoundsLayerTransitionObjectFactory(
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
        .Where(og => og.Type == "LayerTransition")
        .SelectMany(layerConfig => CreateTransitionObjects(
          layerConfig,
          cameraBoundsByLayerName));
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
      ILookup<string, Bounds> cameraBoundsByLayerLookup)
    {
      var vertices = CreateMatrixVertices(layerConfig.TiledLayer);
      var transitionObjectBoundaries = vertices.GetRectangleBounds();

      var prefabName = layerConfig.TiledLayer.GetPropertyValue("Prefab");

      var asset = LoadPrefabAsset(prefabName);

      var transitionsToLayer = layerConfig.TiledLayer.GetPropertyValue("TransitionsTo");

      var cameraBounds = cameraBoundsByLayerLookup[transitionsToLayer];

      foreach (var transitionObjectBounds in transitionObjectBoundaries)
      {
        var intersectingCameraBounds = cameraBounds.Single(bounds => bounds.Intersects(transitionObjectBounds));

        yield return CreateCameraModifier(
          prefabName + " Transition To " + transitionsToLayer + " Camera Modifier",
          transitionsToLayer,
          layerConfig.Universe,
          intersectingCameraBounds,
          transitionObjectBounds);

        yield return CreateInstantiableObject(
          asset,
          prefabName,
          layerConfig,
          new LayerTransitionInstantiationArguments
          {
            TransitionObjectBounds = transitionObjectBounds,
            TransitionToLayer = transitionsToLayer,
          });
      }
    }

    private GameObject CreateCameraModifier(
      string gameObjectName,
      string transitionsToLayer,
      string universe,
      Bounds intersectingCameraBounds,
      Bounds transitionObjectBounds)
    {
      var asset = LoadPrefabAsset("Camera Modifier");
      var layerConfig = new AbstractTiledLayerConfig
      {
        Layer = transitionsToLayer,
        Universe = universe
      };

      var args = new CameraModifierInstantiationArguments
      {
        Bounds = intersectingCameraBounds,
        BoundsPropertyInfos = new CameraModifierInstantiationArguments.BoundsPropertyInfo[]
        {
          new CameraModifierInstantiationArguments.BoundsPropertyInfo { Bounds = transitionObjectBounds } 
        }
      };

      return CreateInstantiableObject(
        asset,
        gameObjectName,
        layerConfig,
        args);
    }
  }
}
