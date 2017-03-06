﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public abstract class AbstractGameObjectFactory
  {
    protected readonly Map Map;

    protected readonly GameObject Root;

    protected readonly Dictionary<string, Objecttype> ObjecttypesByName;

    protected readonly Dictionary<string, string> PrefabLookup = new Dictionary<string, string>();

    protected readonly TiledObjectLayerConfig[] ObjectLayerConfigs;

    protected readonly TiledTileLayerConfig[] TileLayerConfigs;

    protected AbstractGameObjectFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
    {
      Root = root;
      PrefabLookup = prefabLookup;
      ObjecttypesByName = objecttypesByName;

      Map = map;

      TileLayerConfigs = map
        .Layers
        .Select(layer => TiledTileLayerConfigFactory.Create(layer))
        .ToArray();

      ObjectLayerConfigs = map
        .Objectgroup
        .Select(og => new TiledObjectLayerConfig
        {
          TiledObjectgroup = og,
          Type = og.GetPropertyValue("Type"),
          Universe = og.GetPropertyValue("Universe"),
          Commands = og.GetCommands().ToArray()
        })
        .ToArray();
    }

    public abstract IEnumerable<GameObject> Create();

    protected virtual void OnGameObjectCreated(AbstractTiledLayerConfig layerConfig, GameObject gameObject)
    {
    }

    protected MatrixVertices CreateMatrixVertices(Layer layer)
    {
      var tileNumbers = new long[Map.Height * Map.Width];

      var tileNumbersFlipped = layer
        .Data
        .Text
        .Split(',')
        .Select(text => long.Parse(text));

      var rowIndex = Map.Height - 1;
      var columnIndex = 0;

      foreach (var value in tileNumbersFlipped)
      {
        var location = rowIndex * Map.Width + columnIndex;

        tileNumbers[location] = value;

        columnIndex++;

        if (columnIndex == Map.Width)
        {
          columnIndex = 0;
          rowIndex--;
        }
      }

      var matrix = new Matrix<long>(tileNumbers, Map.Height, Map.Width);

      return new MatrixVertices(matrix, Map.Tilewidth, Map.Tileheight);
    }

    protected void AddEdgeColliders(GameObject parent, Vector2[] points, int padding = 0, bool isTrigger = false)
    {
      for (var i = 0; i < points.Length - 1; i++)
      {
        AddEdgeCollider(parent, points[i], points[i + 1], isTrigger);
      }

      AddEdgeCollider(parent, points.Last(), points.First(), isTrigger);
    }

    private void AddEdgeCollider(GameObject parent, Vector2 from, Vector2 to, bool isTrigger)
    {
      var edgeCollider = parent.AddComponent<EdgeCollider2D>();

      edgeCollider.isTrigger = isTrigger;
      edgeCollider.points = new Vector2[] { from, to };
    }

    protected GameObject LoadPrefabAsset(string prefabName)
    {
      string assetPath;

      if (!PrefabLookup.TryGetValue(prefabName, out assetPath))
      {
        throw new MissingReferenceException("No prefab with name '" + prefabName + "' exists at this project");
      }

      return AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
    }

    protected GameObject CreateInstantiableObject<TInstantiationArguments>(
      GameObject asset,
      string gameObjectName,
      AbstractTiledLayerConfig layerConfig,
      TInstantiationArguments arguments,
      string tiledObjectName = null)
      where TInstantiationArguments : AbstractInstantiationArguments
    {
      var gameObject = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity) as GameObject;

      AssignTileToPrefab(layerConfig, tiledObjectName, gameObjectName, gameObject);

      OnGameObjectCreated(layerConfig, gameObject);

      gameObject.name = gameObjectName;

      var instantiable = gameObject.GetComponent<IInstantiable<TInstantiationArguments>>();
      if (instantiable != null)
      {
        instantiable.Instantiate(arguments);
      }
      else
      {
        gameObject.transform.ForEachChildComponent<IInstantiable<TInstantiationArguments>>(
          component => component.Instantiate(arguments));
      }

      return gameObject;
    }

    private void AssignTileToPrefab(
      AbstractTiledLayerConfig layerConfig,
      string tiledObjectName,
      string gameObjectName,
      GameObject gameObject)
    {
      var assignTileToPrefab = layerConfig.Commands.Contains("AssignTileToPrefab");
      if (!assignTileToPrefab)
      {
        return;
      }

      if (tiledObjectName == null)
      {
        throw new ArgumentException("Tiled Object name must be set when assigning tiles to prefabs. Prefab = " + gameObjectName);
      }

      var rootTiledTransform = Root.transform.FindFirstRecursive(tiledObjectName);

      var meshTransform = rootTiledTransform.GetComponentInChildren<MeshRenderer>().transform;

      meshTransform.position = Vector3.zero;
      meshTransform.parent = gameObject.transform;
      meshTransform.gameObject.name = "Tiled Mesh";
      meshTransform.gameObject.layer = LayerMask.NameToLayer("Background");
    }

    protected bool IsFlippedHorizontally(Object obj)
    {
      return obj.Gid >= 2000000000;
    }

    protected bool IsFlippedVertically(Object obj)
    {
      return (obj.Gid >= 1000000000 && obj.Gid < 2000000000) || obj.Gid >= 3000000000;
    }
  }
}
