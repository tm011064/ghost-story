#if UNITY_EDITOR

using UnityEngine;
public partial class Checkpoint : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    transform.position = arguments.TiledRectBounds.center;

    var boxCollider = GetComponent<BoxCollider2D>();
    boxCollider.size = arguments.TiledRectBounds.size;

    PortalName = arguments.TiledObjectName;
  }
}

#endif