#if UNITY_EDITOR

using UnityEngine;

public partial class InventoryItemBehaviour : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    transform.position = arguments.TiledRectBounds.center.AddY(arguments.TiledRectBounds.size.y / 2);

    var boxCollider = gameObject.AddComponent<BoxCollider2D>();

    boxCollider.isTrigger = true;
    boxCollider.size = arguments.TiledRectBounds.size;

    var meshRenderer = GetComponentInChildren<MeshRenderer>();
    meshRenderer.transform.position = transform.position
      .AddX(-arguments.TiledRectBounds.size.x / 2)
      .AddY(arguments.TiledRectBounds.size.y / 2);
  }
}

#endif
