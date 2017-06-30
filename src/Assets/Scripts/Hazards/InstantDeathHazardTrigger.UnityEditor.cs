#if UNITY_EDITOR

using UnityEngine;

public partial class InstantDeathHazardTrigger : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    transform.position = arguments.TiledRectBounds.center;

    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();
    boxCollider.size = arguments.TiledRectBounds.size;
  }
}

#endif