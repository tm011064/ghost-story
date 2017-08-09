using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PrefabInstantiationArguments : AbstractInstantiationArguments
{
  public Bounds TiledRectBounds;

  public IEnumerable<Bounds> IntersectingCameraBounds;

  public IEnumerable<CameraTriggerBounds> IntersectingCameraTriggerBounds;

  public IEnumerable<Bounds> WrappingCameraBounds;

  public Dictionary<string, string> Properties;

  public string TiledObjectName;

  public void CheckHasTiledObjectName()
  {
    if (string.IsNullOrEmpty(TiledObjectName))
    {
      throw new ArgumentNullException("TiledObjectName", new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name);
    }
  }

  private void HasKey(string key, string caller)
  {
    if (!Properties.ContainsKey(key))
    {
      throw new KeyNotFoundException(caller + " missing property '" + key.ToString() + "'");
    }
  }

  public void CheckHasProperty(string key)
  {
    HasKey(key, "'" + new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name + "::" + (TiledObjectName ?? "NULL") + "'");
  }

  public void CheckHasProperties(params string[] keys)
  {
    var caller = "'" + new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name + "::" + (TiledObjectName ?? "NULL") + "'";
    foreach (var key in keys)
    {
      HasKey(key, caller);
    }
  }

  public bool TryBooleanProperty(string key, bool defaultValue = false)
  {
    var parsed = defaultValue;

    string value;
    if (Properties.TryGetValue(key, out value))
    {
      bool.TryParse(value, out parsed);
    }

    return parsed;
  }

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
