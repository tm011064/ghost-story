#if UNITY_EDITOR

using UnityEngine;

public partial class SaveGameArea : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    arguments.CheckHasTiledObjectName();

    PortalName = arguments.TiledObjectName;

    transform.position = arguments.TiledRectBounds.center;

    var boxCollider = GetComponent<BoxCollider2D>();
    boxCollider.size = arguments.TiledRectBounds.size;
  }
}

#endif