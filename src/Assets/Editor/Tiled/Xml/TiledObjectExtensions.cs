using System;
using System.Collections.Generic;
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

    public static Dictionary<string, string> GetProperties(this TiledObject tiledObject, Dictionary<string, ObjectType> objectTypesByName)
    {
      var properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

      ObjectType objectType;

      if (!string.IsNullOrEmpty(tiledObject.Type)
        && objectTypesByName.TryGetValue(tiledObject.Type, out objectType))
      {
        foreach (var property in objectType.Properties)
        {
          properties[property.Name] = property.Default;
        }
      }

      if (tiledObject.PropertyGroup != null)
      {
        foreach (var property in tiledObject.PropertyGroup.Properties)
        {
          properties[property.Name] = property.Value;
        }
      }

      return properties;
    }

    public static bool HasProperty(this TiledObject tiledObject, string propertyName, Dictionary<string, ObjectType> objectTypesByName)
    {
      return GetProperties(tiledObject, objectTypesByName)
        .ContainsKey(propertyName);
    }

    public static bool HasProperty(this TiledObject tiledObject, string propertyName, string propertyValue, Dictionary<string, ObjectType> objectTypesByName)
    {
      var properties = GetProperties(tiledObject, objectTypesByName);

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

    public static bool IsType(this TiledObject tiledObject, string typeName)
    {
      return string.Equals(tiledObject.Type, typeName, StringComparison.OrdinalIgnoreCase);
    }
  }
}
