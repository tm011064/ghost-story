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
      AbstractGameObjectFactory[] gameObjectFactories = null,
      Property[] propertyFilters = null)
    {
      if (gameObjectFactories == null)
      {
        gameObjectFactories = CreateDefaultGameObjectFactories().ToArray();
      }

      var tiledObjectsGameObject = new GameObject("Tiled Objects");
      tiledObjectsGameObject.transform.position = Vector3.zero;

      tiledObjectsGameObject.AttachChildren(
        gameObjectFactories.SelectMany(f => f.Create(propertyFilters ?? new Property[0])));

      AssignTags(parent);

      tiledObjectsGameObject.transform.parent = parent.transform;
    }

    private void AssignTags(GameObject prefab)
    {
      foreach (var layer in Map.ForEachLayerWithPropertyName("Tag"))
      {
        var tag = layer.GetProperty("Tag");
        var transform = prefab.transform.FindChild(layer.Name);

        transform.tag = tag;

        Debug.Log("Tile2Unity Import: Assigned tag '" + tag + "' to game object " + transform);
      }
    }

    private IEnumerable<AbstractGameObjectFactory> CreateDefaultGameObjectFactories()
    {
      yield return new PlatformColliderFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new OneWayPlatformColliderFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new DeathHazardFactory(Map, PrefabLookup, ObjecttypesByName);
      yield return new LayerPrefabFactory(Map, PrefabLookup, ObjecttypesByName);
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
