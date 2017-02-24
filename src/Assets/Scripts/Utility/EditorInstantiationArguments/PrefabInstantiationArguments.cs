using System.Collections.Generic;
using UnityEngine;

public class PrefabInstantiationArguments : AbstractInstantiationArguments
{
  public Bounds TiledRectBounds;

  public IEnumerable<Bounds> IntersectingCameraBounds;

  public IEnumerable<Bounds> WrappingCameraBounds;

  public Dictionary<string, string> Properties;

  public string TiledObjectName;
}
