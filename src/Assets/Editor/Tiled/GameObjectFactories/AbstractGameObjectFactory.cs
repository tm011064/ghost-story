using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public abstract class AbstractGameObjectFactory
  {
    protected readonly Map Map;

    protected readonly Dictionary<string, Objecttype> ObjecttypesByName;

    protected readonly Dictionary<string, string> PrefabLookup = new Dictionary<string, string>();

    protected readonly TiledObjectLayerConfig[] ObjectLayerConfigs;

    protected readonly TiledTileLayerConfig[] TileLayerConfigs;

    protected AbstractGameObjectFactory(
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
    {
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
          Layer = og.GetPropertyValue("Layer"),
          Type = og.GetPropertyValue("Type"),
          Universe = og.GetPropertyValue("Universe")
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
      TInstantiationArguments arguments)
      where TInstantiationArguments : AbstractInstantiationArguments
    {
      var gameObject = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity) as GameObject;

      OnGameObjectCreated(layerConfig, gameObject);

      gameObject.name = gameObjectName;

      var instantiable = gameObject.GetComponentOrThrow<IInstantiable<TInstantiationArguments>>();

      instantiable.Instantiate(arguments);

      return gameObject;
    }
  }
}
