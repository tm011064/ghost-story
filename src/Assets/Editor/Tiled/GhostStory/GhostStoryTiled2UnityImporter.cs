using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using Tiled2Unity;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  [CustomTiledImporter]
  class GhostStoryTiled2UnityImporter : ICustomTiledImporter
  {
    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props)
    {
    }

    public void CustomizePrefab(GameObject prefab)
    {
      CustomizeSafe(prefab);
    }

    private void Customize(GameObject prefab)
    {
      AttachCustomObjects(prefab);
      LinkCheckpointsToRooms(prefab);
    }

    private void CustomizeSafe(GameObject prefab)
    {
      try
      {
        Customize(prefab);
      }
      catch (Exception ex)
      {
        Debug.LogError(ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace ?? string.Empty);
      }
    }

    private void LinkCheckpointsToRooms(GameObject prefab)
    {
      var checkpoints = prefab.GetComponentsInChildren<Checkpoint>();
      var rooms = prefab.GetComponentsInChildren<FullScreenScroller>();
      var cameraModifiers = prefab.GetComponentsInChildren<CameraModifier>();

      foreach (var checkpoint in checkpoints)
      {
        var roomTransforms = rooms
          .Where(r => r.Contains(checkpoint.transform.position))
          .Select(r => r.transform);

        var cameraModifierTransforms = cameraModifiers
          .Where(c => c.Contains(checkpoint.transform.position))
          .Select(c => c.transform);

        var parentTransform = roomTransforms.Union(cameraModifierTransforms).FirstOrDefault();

        if (parentTransform == null)
        {
          throw new Exception("Checkpoint " + checkpoint.name + " must be within a room");
        }

        checkpoint.transform.parent = parentTransform;
      }
    }

    private void AttachCustomObjects(GameObject prefab)
    {
      var scene = EditorSceneManager.GetActiveScene();

      var directory = Path.GetDirectoryName(scene.path);

      var tmxFile = Path.Combine(
        directory,
        prefab.name + ".tmx");

      var objectTypesPath = "Assets/Tiled/objecttypes.xml";

      Debug.Log("Importing file '" + tmxFile + "'");

      var importer = TiledProjectImporterFactory.CreateFromFile(
        tmxFile,
        objectTypesPath);

      importer.Import(
        prefab,
        new AbstractGameObjectFactory[]
        {
          new GhostStoryPlatformColliderFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new GhostStoryCamerBoundsTransitionObjectFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new GhostStoryCameraModifierFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new GhostStoryTiledObjectPrefabFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new GhostStoryTiledLayerPrefabFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new GhostStoryCamerBoundsLayerTransitionObjectFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjecttypesByName)
        });

      AssignLevelObjectConfigs(prefab, importer.Map);
    }

    private void AssignLevelObjectConfigs(GameObject prefab, Map map)
    {
      var layerTypes = new string[] { "Background", "Platform", "OneWayPlatform", "Transition" };

      foreach (var layer in map.Layers)
      {
        var type = layer.GetPropertyValue("Type");
        if (layerTypes.Contains(type))
        {
          var layerConfig = TiledTileLayerConfigFactory.Create(layer);

          var transform = prefab.transform.FindChild(layerConfig.TiledLayer.Name);

          transform.gameObject.AddLevelConfigComponent(layerConfig);
          transform.gameObject.layer = LayerMask.NameToLayer("Background");
        }
      }
    }
  }
}
