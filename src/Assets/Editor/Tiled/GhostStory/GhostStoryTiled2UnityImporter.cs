using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
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
          new GhostStoryPlatformColliderFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjectTypesByName),
          new GhostStoryCameraModifierFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjectTypesByName),
          new GhostStoryOneWayPlatformColliderFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjectTypesByName),
          new GhostStoryTiledLayerPrefabFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjectTypesByName),
          new GhostStoryTiledObjectPrefabFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjectTypesByName),
          new GhostStoryEdgeColliderFactory(prefab, importer.Map, importer.PrefabLookup, importer.ObjectTypesByName)
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

          var transform = prefab.transform.Find(layerConfig.TiledLayer.Name);

          if (transform != null)
          {
            transform.gameObject.AddLevelConfigComponent(layerConfig);
            transform.gameObject.layer = LayerMask.NameToLayer("Background");
          }
        }
      }
    }
  }
}
