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
    private readonly Map _map;

    private readonly Objecttypes _objecttypes;

    private readonly Dictionary<string, Objecttype> _objecttypesByName;

    private readonly Dictionary<string, string> _prefabLookup;

    public TiledProjectImporter(Map map, Objecttypes objecttypes)
    {
      _map = map;

      _objecttypesByName = objecttypes
        .Objecttype
        .ToDictionary(ot => ot.Name, ot => ot, StringComparer.InvariantCultureIgnoreCase);

      _prefabLookup = AssetDatabase
        .GetAllAssetPaths()
        .Where(path => path.EndsWith(".prefab"))
        .ToDictionary(p => GetPrefabName(p), p => p, StringComparer.InvariantCultureIgnoreCase);
    }

    public void Import(
      GameObject parent = null,
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

      if (parent != null)
      {
        tiledObjectsGameObject.transform.parent = parent.transform;
      }
    }

    private IEnumerable<AbstractGameObjectFactory> CreateDefaultGameObjectFactories()
    {
      yield return new PlatformColliderFactory(_map, _prefabLookup, _objecttypesByName);
      yield return new OneWayPlatformColliderFactory(_map, _prefabLookup, _objecttypesByName);
      yield return new DeathHazardFactory(_map, _prefabLookup, _objecttypesByName);
      yield return new LayerPrefabFactory(_map, _prefabLookup, _objecttypesByName);
      yield return new TiledObjectPrefabFactory(_map, _prefabLookup, _objecttypesByName);
      yield return new CameraModifierFactory(_map, _prefabLookup, _objecttypesByName);
    }

    private string GetPrefabName(string assetPath)
    {
      var fileInfo = new FileInfo(assetPath);

      return fileInfo.Name.Remove(fileInfo.Name.Length - (".prefab".Length));
    }
  }
}
