using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Editor.Tiled.Xml;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public abstract class AbstractGameObjectFactory
  {
    protected readonly Map Map;

    protected readonly GameObject Root;

    protected readonly Dictionary<string, string> PrefabLookup = new Dictionary<string, string>();

    protected readonly TiledObjectLayerConfig[] ObjectLayerConfigs;

    protected readonly TiledGroupConfig[] GroupConfigs;

    protected readonly TiledTileLayerConfig[] TileLayerConfigs;

    protected AbstractGameObjectFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup)
    {
      Root = root;
      PrefabLookup = prefabLookup;
      Map = map;

      TileLayerConfigs = map
        .AllLayers()
        .Select(TiledTileLayerConfigFactory.Create)
        .ToArray();

      ObjectLayerConfigs = map
        .AllObjectGroups()
        .Select(CreateTiledObjectGroupConfig)
        .ToArray();

      GroupConfigs = map
        .AllGroupsDepthFirst()
        .Select(CreateGroupConfig)
        .ToArray();
    }

    private TiledObjectLayerConfig CreateTiledObjectGroupConfig(ObjectGroup objectGroup)
    {
      return new TiledObjectLayerConfig
      {
        TiledObjectGroup = objectGroup,
        Type = objectGroup.GetPropertyValue("Type"),
        Universe = objectGroup.GetPropertyValue("Universe"),
        Commands = objectGroup.GetCommands().ToArray()
      };
    }

    private TiledGroupConfig CreateGroupConfig(Group group)
    {
      var config = new TiledGroupConfig
      {
        Group = group,
        Type = group.TryGetProperty("Type"),
        Universe = group.TryGetProperty("Universe"),
        Commands = group.GetCommands().ToArray()
      };

      config.ObjectLayerConfigs = group
        .ObjectGroups
        .Select(CreateTiledObjectGroupConfig)
        .ToArray();

      config.TileLayerConfigs = group
        .Layers
        .Select(TiledTileLayerConfigFactory.Create)
        .ToArray();

      config.GroupConfigs = group
        .Groups
        .Select(CreateGroupConfig)
        .ToArray();

      return config;
    }

    public abstract IEnumerable<GameObject> Create();

    protected virtual void OnGameObjectCreated(AbstractTiledLayerConfig layerConfig, GameObject gameObject)
    {
    }

    protected IEnumerable<GameObject> CreateColliderObjects(Layer layer, string layerName = "Platforms")
    {
      var vertices = CreateMatrixVertices(layer);
      var colliders = vertices.GetColliderEdges();

      foreach (var points in colliders)
      {
        var obj = new GameObject("Collider");

        obj.transform.position = Vector3.zero;
        obj.layer = LayerMask.NameToLayer(layerName);

        AddEdgeColliders(obj, points);

        yield return obj;
      }
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

      return new MatrixVertices(matrix, Map.TileWidth, Map.TileHeight);
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

    protected bool IsFlippedHorizontally(TiledObject obj)
    {
      return obj.Gid >= 2000000000;
    }

    protected bool IsFlippedVertically(TiledObject obj)
    {
      return (obj.Gid >= 1000000000 && obj.Gid < 2000000000) || obj.Gid >= 3000000000;
    }
  }
}
