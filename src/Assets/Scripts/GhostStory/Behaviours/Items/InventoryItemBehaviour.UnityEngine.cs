#if UNITY_EDITOR

using UnityEngine;
public partial class InventoryItemBehaviour : IInstantiable<InstantiationArguments>
{
  public void Instantiate(InstantiationArguments arguments)
  {
    transform.position = arguments.Bounds.center.AddY(arguments.Bounds.size.y / 2);

    var boxCollider = gameObject.AddComponent<BoxCollider2D>();

    boxCollider.isTrigger = true;
    boxCollider.size = arguments.Bounds.size;

    var meshRenderer = GetComponentInChildren<MeshRenderer>();
    meshRenderer.transform.position = transform.position
      .AddX(-arguments.Bounds.size.x / 2)
      .AddY(arguments.Bounds.size.y / 2);
  }
}

#endif
