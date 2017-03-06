#if UNITY_EDITOR

using System.Linq;
using UnityEngine;

public partial class EnemySpawnManager : SpawnBucketItemBehaviour, IInstantiable<PrefabInstantiationArguments>
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;

  private Vector3 _gizmoCenter = Vector3.zero;

  private Vector3 _gizmoExtents = new Vector3(16, 16, 0);

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      if (_enemyToSpawnPrefab != null)
      {
        var boxCollider2D = _enemyToSpawnPrefab.GetComponent<BoxCollider2D>();

        if (boxCollider2D != null)
        {
          _gizmoCenter = boxCollider2D.offset;

          _gizmoExtents = boxCollider2D.size / 2;
        }
      }

      GizmoUtility.DrawBoundingBox(transform.TransformPoint(_gizmoCenter), _gizmoExtents, OutlineGizmoColor);
    }
  }

  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    SpawnArguments = arguments
      .Properties
      .Select(kvp => new SpawnArgument { Key = kvp.Key, Value = kvp.Value })
      .ToArray();

    transform.position = arguments.TiledRectBounds.center;

    transform.ForEachChildComponent<IInstantiable<PrefabInstantiationArguments>>(
      instantiable => instantiable.Instantiate(arguments));
  }
}

#endif