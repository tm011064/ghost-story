using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class CameraModifierFactory : AbstractGameObjectFactory
  {
    public CameraModifierFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(map, prefabLookup, objecttypesByName)
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
      var bounds = layerConfig.TiledObjectgroup.GetTypeOrThrow("Camera Bounds");
      var triggers = layerConfig.TiledObjectgroup.GetTypesOrThrow("Camera Trigger");

      var fullScreenScrollers = triggers.Where(o => o.HasProperty(
        "Triggers Room Transition",
        "true",
        ObjecttypesByName)).ToArray();

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
      Object[] triggers,
      Object boundsObject,
      string prefabName,
      AbstractTiledLayerConfig layerConfig)
    {
      var boundsPropertyInfos = triggers
        .Where(t => t.PolyLine == null)
        .Select(t => new CameraModifierInstantiationArguments.BoundsPropertyInfo
          {
            Bounds = t.GetBounds(),
            Properties = t.GetProperties(ObjecttypesByName)
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
