#if UNITY_EDITOR

using UnityEngine;

public partial class BoxColliderTriggerEnterBehaviour : IInstantiable<CameraModifierInstantiationArguments>, IInstantiable<InstantiationArguments>
{
  public void Instantiate(CameraModifierInstantiationArguments arguments)
  {
    DoInstantiate(arguments);
  }

  public void Instantiate(InstantiationArguments arguments)
  {
    DoInstantiate(arguments);
  }

  private void DoInstantiate<TInstantiationArguments>(TInstantiationArguments instantiationArguments)
    where TInstantiationArguments : InstantiationArguments
  {
    var width = instantiationArguments.Bounds.size.x;
    var height = instantiationArguments.Bounds.size.y;

    var boxCollider = this.GetComponentOrThrow<BoxCollider2D>();

    boxCollider.size = new Vector2(width, height);
  }
}

#endif
