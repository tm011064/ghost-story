using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using Tiled2Unity;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Editor.Tiled
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
          new PlatformColliderFactory(
            importer.Map, 
            importer.PrefabLookup, 
            importer.ObjecttypesByName, 
            "RealWorldPlatforms", 
            "Platforms"),
          new PlatformColliderFactory(
            importer.Map, 
            importer.PrefabLookup, 
            importer.ObjecttypesByName,
            "AlternateWorldPlatforms",
            "AlternatePlatforms"),
          new OneWayPlatformColliderFactory(importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new DeathHazardFactory(importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new LayerPrefabFactory(importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new TiledObjectPrefabFactory(importer.Map, importer.PrefabLookup, importer.ObjecttypesByName),
          new CameraModifierFactory(importer.Map, importer.PrefabLookup, importer.ObjecttypesByName)          
        });
      // TODO (Roman): set up levelsettings
      importer.Import(
        prefab,
        new AbstractGameObjectFactory[]
        {
          new CamerBoundsTransitionObjectFactory(
              importer.Map,
              importer.PrefabLookup,
              importer.ObjecttypesByName,
              "Green Door")
        },
        new Property[] { new Property { Name = "Tag", Value = "RealWorld" } });
    }
  }
}
