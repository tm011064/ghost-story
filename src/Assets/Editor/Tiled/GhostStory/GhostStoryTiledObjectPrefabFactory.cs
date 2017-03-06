using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  public class GhostStoryTiledObjectPrefabFactory : TiledObjectPrefabFactory
  {
    public GhostStoryTiledObjectPrefabFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(root, map, prefabLookup, objecttypesByName)
    {
    }

    protected ILookup<string, Bounds> CreateCameraBoundsByUniverseNameLookup()
    {
      return ObjectLayerConfigs
        .Where(config => config.Type == "CameraBounds")
        .Select(config =>
          new
          {
            Universe = config.Universe,
            Bounds = config.TiledObjectgroup.Object.Select(o => o.GetBounds())
          })
        .SelectMany(c => c.Bounds.Select(b => new { Universe = c.Universe, Bounds = b }))
        .Union(ObjectLayerConfigs
          .Where(config => config.Type == "CameraModifier")
          .Select(config =>
            new
            {
              Universe = config.Universe,
              Bounds = config.TiledObjectgroup.Object.Where(o => o.Type == "Camera Bounds").Select(o => o.GetBounds())
            })
          .SelectMany(c => c.Bounds.Select(b => new { Universe = c.Universe, Bounds = b })))
        .ToLookup(c => c.Universe, c => c.Bounds);
    }

    protected override void OnGameObjectCreated(AbstractTiledLayerConfig layerConfig, GameObject gameObject)
    {
      gameObject.AddLevelConfigComponent(layerConfig);
    }

    protected override IEnumerable<GameObject> CreatePrefabFromGameObject(TiledObjectLayerConfig layerConfig)
    {
      var cameraBoundsByUniverseNameLookup = CreateCameraBoundsByUniverseNameLookup();

      foreach (var obj in layerConfig.TiledObjectgroup.Object)
      {
        var properties = obj.GetProperties(ObjecttypesByName);

        var prefabName = properties["Prefab"];

        var asset = LoadPrefabAsset(prefabName);

        var cameraBounds = cameraBoundsByUniverseNameLookup[layerConfig.Universe];

        var intersectingCameraBounds = cameraBounds
          .Where(bounds => bounds.Intersects(obj.GetBounds()))
          .ToArray();

        var wrappingCameraBounds = cameraBounds
          .Where(bounds => bounds.Contains(obj.GetBounds().center))
          .ToArray();

        var instantiationArguments = new PrefabInstantiationArguments
        {
          IntersectingCameraBounds = intersectingCameraBounds,
          PrefabsAssetPathsByShortName = PrefabLookup,
          Properties = properties,
          TiledObjectName = obj.Name,
          TiledRectBounds = obj.GetBounds(),
          WrappingCameraBounds = wrappingCameraBounds
        };

        yield return CreateInstantiableObject(
          asset,
          prefabName,
          layerConfig,
          instantiationArguments,
          obj.Name);
      }
    }
  }
}
