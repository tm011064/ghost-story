using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class TiledProjectImporter
  {
    public readonly Map Map;

    public readonly Dictionary<string, ObjectType> ObjectTypesByName;

    public readonly Dictionary<string, string> PrefabLookup;

    public TiledProjectImporter(Map map, ObjectTypeGroup group)
    {
      Map = map;

      ObjectTypesByName = group
        .ObjectTypes
        .ToDictionary(ot => ot.Name, ot => ot, StringComparer.InvariantCultureIgnoreCase);

      PrefabLookup = AssetDatabase
        .GetAllAssetPaths()
        .Where(path => path.EndsWith(".prefab"))
        .ToDictionary(p => GetPrefabName(p), p => p, StringComparer.InvariantCultureIgnoreCase);
    }

    public void Import(
      GameObject parent,
      AbstractGameObjectFactory[] gameObjectFactories = null)
    {
      if (gameObjectFactories == null)
      {
        gameObjectFactories = CreateDefaultGameObjectFactories(parent).ToArray();
      }

      var tiledObjectsGameObject = new GameObject("Tiled Objects");
      tiledObjectsGameObject.transform.position = Vector3.zero;

      tiledObjectsGameObject.AttachChildren(
        gameObjectFactories.SelectMany(f => f.Create()));

      tiledObjectsGameObject.transform.parent = parent.transform;

      ExecuteCommands(parent);
    }

    private void ExecuteCommands(GameObject prefab)
    {
      var objectCommands = Map
        .ForEachObjectGroupWithPropertyName("Commands")
        .Select(og => new { Name = og.Name, Commands = og.GetCommands() });

      var layerCommands = Map
        .ForEachLayerWithPropertyName("Commands")
        .Select(layer => new { Name = layer.Name, Commands = layer.GetCommands() });

      foreach (var command in objectCommands.Concat(layerCommands))
      {
        ExecuteDestroyPrefabCommand(prefab, command.Name, command.Commands);
      }
    }

    private void ExecuteDestroyPrefabCommand(GameObject prefab, string gameObjectName, IEnumerable<string> commands)
    {
      if (!commands.Any(c => string.Equals(c, "DestroyPrefab", StringComparison.OrdinalIgnoreCase)))
      {
        return;
      }

      Destroy(prefab, gameObjectName);
    }

    private void Destroy(GameObject prefab, string name)
    {
      var childTransform = prefab.transform.FindChild(name);

      while (childTransform != null)
      {
        Debug.Log("Tile2Unity Import: Destroying game object " + name);

        UnityEngine.Object.DestroyImmediate(childTransform.gameObject);

        childTransform = prefab.transform.FindChild(name);
      }
    }

    private IEnumerable<AbstractGameObjectFactory> CreateDefaultGameObjectFactories(GameObject parent)
    {
      yield return new PlatformColliderFactory(parent, Map, PrefabLookup, ObjectTypesByName);
      yield return new OneWayPlatformColliderFactory(parent, Map, PrefabLookup, ObjectTypesByName);
      yield return new DeathHazardFactory(parent, Map, PrefabLookup, ObjectTypesByName);
      yield return new TiledLayerPrefabFactory(parent, Map, PrefabLookup, ObjectTypesByName);
      yield return new TiledObjectPrefabFactory(parent, Map, PrefabLookup, ObjectTypesByName);
      yield return new CameraModifierFactory(parent, Map, PrefabLookup, ObjectTypesByName);
    }

    private string GetPrefabName(string assetPath)
    {
      var fileInfo = new FileInfo(assetPath);

      return fileInfo.Name.Remove(fileInfo.Name.Length - (".prefab".Length));
    }
  }
}
