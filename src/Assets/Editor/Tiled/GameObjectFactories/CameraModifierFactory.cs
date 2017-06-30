using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class CameraModifierFactory : AbstractGameObjectFactory
  {
    public CameraModifierFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup)
      : base(root, map, prefabLookup)
    {
    }

    public override IEnumerable<GameObject> Create()
    {
      return ObjectLayerConfigs
        .Where(config => config.Type == "CameraModifier")
        .SelectMany(config => CreateCameraModifers(config));
    }

    private IEnumerable<GameObject> CreateCameraModifers(TiledObjectLayerConfig layerConfig)
    {
      var bounds = layerConfig.TiledObjectGroup.GetTiledObjectOrThrow("Camera Bounds");
      var triggers = layerConfig.TiledObjectGroup.GetTiledObjectsOrThrow("Camera Trigger");

      var fullScreenScrollers = triggers
        .Where(o => o.HasProperty("Triggers Room Transition", "true"))
        .ToArray();

      if (fullScreenScrollers.Any())
      {
        yield return CreateCameraModifier(
          fullScreenScrollers,
          bounds,
          "Full Screen Scroller",
          layerConfig);
      }

      var cameraModifiers = triggers.Except(fullScreenScrollers).ToArray();

      if (cameraModifiers.Any())
      {
        yield return CreateCameraModifier(
          cameraModifiers,
          bounds,
          "Camera Modifier",
          layerConfig);
      }
    }

    private GameObject CreateCameraModifier(
      TiledObject[] triggers,
      TiledObject boundsObject,
      string prefabName,
      AbstractTiledLayerConfig layerConfig)
    {
      var boundsPropertyInfos = triggers
        .Where(t => t.PolyLine == null)
        .Select(t => new CameraModifierInstantiationArguments.BoundsPropertyInfo
        {
          Bounds = t.GetBounds(),
          Properties = t.GetProperties()
        })
        .ToArray();

      var asset = LoadPrefabAsset(prefabName);

      return CreateInstantiableObject(
       asset,
       prefabName,
       layerConfig,
       new CameraModifierInstantiationArguments
       {
         BoundsPropertyInfos = boundsPropertyInfos,
         Bounds = boundsObject.GetBounds()
       });
    }
  }
}
