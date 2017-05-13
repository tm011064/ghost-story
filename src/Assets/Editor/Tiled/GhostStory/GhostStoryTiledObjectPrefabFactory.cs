using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  public class GhostStoryTiledObjectPrefabFactory : TiledObjectPrefabFactory
  {
    public GhostStoryTiledObjectPrefabFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, ObjectType> objectTypesByName)
      : base(root, map, prefabLookup, objectTypesByName)
    {
    }

    protected ILookup<string, CameraTriggerBounds> CreateTriggerBoundsByUniverseNameLookup()
    {
      return ObjectLayerConfigs
          .Where(config => config.Type == "CameraModifier")
          .Select(config =>
            new
            {
              Universe = config.Universe,
              CameraBounds = config.TiledObjectGroup.Objects.Single(o => o.Type == "Camera Bounds").GetBounds(),
              TriggerBounds = config.TiledObjectGroup.Objects.Where(o => o.Type == "Camera Trigger").Select(o => o.GetBounds())
            })
          .SelectMany(c => c.TriggerBounds.Select(
            b => new
            {
              Universe = c.Universe,
              CameraTriggerBounds = new CameraTriggerBounds { CameraBounds = c.CameraBounds, TriggerBounds = b }
            }))
          .ToLookup(c => c.Universe, c => c.CameraTriggerBounds);
    }

    protected ILookup<string, Bounds> CreateCameraBoundsByUniverseNameLookup()
    {
      return ObjectLayerConfigs
        .Where(config => config.Type == "CameraBounds")
        .Select(config =>
          new
          {
            Universe = config.Universe,
            Bounds = config.TiledObjectGroup.Objects.Select(o => o.GetBounds())
          })
        .SelectMany(c => c.Bounds.Select(b => new { Universe = c.Universe, Bounds = b }))
        .Union(ObjectLayerConfigs
          .Where(config => config.Type == "CameraModifier")
          .Select(config =>
            new
            {
              Universe = config.Universe,
              Bounds = config.TiledObjectGroup.Objects.Where(o => o.Type == "Camera Bounds").Select(o => o.GetBounds())
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
      var triggerBoundsByUniverseNameLookup = CreateTriggerBoundsByUniverseNameLookup();

      foreach (var obj in layerConfig.TiledObjectGroup.Objects)
      {
        var properties = obj.GetProperties(ObjectTypesByName);

        var prefabName = properties["Prefab"];

        var asset = LoadPrefabAsset(prefabName);

        var cameraBounds = cameraBoundsByUniverseNameLookup[layerConfig.Universe];
        var triggerBounds = triggerBoundsByUniverseNameLookup[layerConfig.Universe];

        var intersectingCameraBounds = cameraBounds
          .Where(bounds => bounds.Intersects(obj.GetBounds()))
          .ToArray();

        var intersectingCameraTriggerBounds = triggerBounds
          .Where(b => b.TriggerBounds.Intersects(obj.GetBounds()))
          .ToArray();

        var wrappingCameraBounds = cameraBounds
          .Where(bounds => bounds.Contains(obj.GetBounds().center))
          .ToArray();

        var instantiationArguments = new PrefabInstantiationArguments
        {
          IntersectingCameraBounds = intersectingCameraBounds,
          IntersectingCameraTriggerBounds = intersectingCameraTriggerBounds,
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
