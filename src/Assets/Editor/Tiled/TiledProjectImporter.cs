using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Editor.Tiled.GameObjectFactories;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public class TiledProjectImporter
  {
    public readonly Map Map;

    public readonly Dictionary<string, Objecttype> ObjecttypesByName;

    public readonly Dictionary<string, string> PrefabLookup;

    public TiledProjectImporter(Map map, Objecttypes objecttypes)
    {
      Map = map;

      ObjecttypesByName = objecttypes
        .Objecttype
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
        gameObjectFactories = CreateDefaultGameObjectFactories().ToArray();
      }

      ExecuteCommands(parent);

      var tiledObjectsGameObject = new GameObject("Tiled Objects");
      tiledObjectsGameObject.transform.position = Vector3.zero;

      tiledObjectsGameObject.AttachChildren(
        gameObjectFactories.SelectMany(f => f.Create()));

      tiledObjectsGameObject.transform.parent = parent.transform;
    }

    private void ExecuteCommands(GameObject prefab)
    {
      var objectCommands = Map
        .ForEachObjectGroupWithPropertyName("Commands")
        .Select(og => new { Name = og.Name, Commands = GetCommands(og) });

      var layerCommands = Map
        .ForEachLayerWithPropertyName("Commands")
        .Select(layer => new { Name = layer.Name, Commands = GetCommands(layer) });

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

    private IEnumerable<string> GetCommands(Objectgroup objectgroup)
    {
      return objectgroup.GetPropertyValue("Commands")
        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());
    }

    private IEnumerable<string> GetCommands(Layer layer)
    {
      return layer.GetPropertyValue("Commands")
        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());
    }

    private IEnumerable<AbstractGameObjectFactory> CreateDefaultGameObjectFactories()
    {
      yield return new PlatformColliderFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new OneWayPlatformColliderFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new DeathHazardFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new TiledLayerPrefabFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new TiledObjectPrefabFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new CameraModifierFactory(Map, PrefabLookup, ObjecttypesByName);
    }

    private string GetPrefabName(string assetPath)
    {
      var fileInfo = new FileInfo(assetPath);

      return fileInfo.Name.Remove(fileInfo.Name.Length - (".prefab".Length));
    }
  }
}
