using System.Collections.Generic;
using UnityEngine;


public class CameraTriggerBounds // TODO (Roman): move
{
  public Bounds CameraBounds;

  public Bounds TriggerBounds;
}

public class PrefabInstantiationArguments : AbstractInstantiationArguments
{
  public Bounds TiledRectBounds;

  public IEnumerable<Bounds> IntersectingCameraBounds;

  public IEnumerable<CameraTriggerBounds> IntersectingCameraTriggerBounds;

  public IEnumerable<Bounds> WrappingCameraBounds;

  public Dictionary<string, string> Properties;

  public string TiledObjectName;

  public string TryProperty(string key, string defaultValue = null)
  {
    string value;
    if (Properties.TryGetValue(key, out value))
    {
      return value;
    }

    return defaultValue;
  }
}
