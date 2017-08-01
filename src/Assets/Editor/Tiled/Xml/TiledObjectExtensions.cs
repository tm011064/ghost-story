using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.Xml
{
  public static class TiledObjectExtensions
  {
    public static IEnumerable<T> Get<T>(this IEnumerable<TiledObject> tiledObjects, Func<TiledObject, T> func)
    {
      foreach (var obj in tiledObjects)
      {
        yield return func(obj);
      }
    }

    public static Dictionary<string, string> GetProperties(this TiledObject tiledObject)
    {
      return tiledObject.PropertyGroup == null
        ? new Dictionary<string, string>()
        : tiledObject.PropertyGroup.Properties.ToDictionary(p => p.Name, p => p.Value);
    }

    public static bool HasProperty(this TiledObject tiledObject, string propertyName)
    {
      return GetProperties(tiledObject)
        .ContainsKey(propertyName);
    }

    public static bool HasProperty(this TiledObject tiledObject, string propertyName, string propertyValue)
    {
      var properties = GetProperties(tiledObject);

      string value;

      return properties.TryGetValue(propertyName, out value)
        && string.Equals(propertyValue, value, StringComparison.OrdinalIgnoreCase);
    }

    public static Bounds GetBounds(this TiledObject tiledObject)
    {
      if (tiledObject.IsImage())
      {
        return new Bounds(
          new Vector2(tiledObject.X + tiledObject.Width / 2, -tiledObject.Y),
          new Vector2(tiledObject.Width, tiledObject.Height));
      }

      return new Bounds(
          new Vector2(tiledObject.X + tiledObject.Width / 2, -(tiledObject.Y + tiledObject.Height / 2)),
          new Vector2(tiledObject.Width, tiledObject.Height));
    }

    public static bool IsImage(this TiledObject tiledObject)
    {
      return tiledObject.Gid.HasValue;
    }

    public static bool IsCollider(this TiledObject tiledObject)
    {
      return !tiledObject.Gid.HasValue;
    }
  }
}
