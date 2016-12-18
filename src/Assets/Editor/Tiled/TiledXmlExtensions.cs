﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled
{
  public static class TiledXmlExtensions
  {
    public static Vector2[] ToVectors(this PolyLine polyLine)
    {
      var coordinates = polyLine.Points.Split(' ');

      var startPoints = coordinates[0].Split(',');
      var endPoints = coordinates[1].Split(',');

      return new Vector2[]
      {
        new Vector2(int.Parse(startPoints[0]), int.Parse(startPoints[1])),
        new Vector2(int.Parse(endPoints[0]), int.Parse(endPoints[1]))
      };
    }

    public static Dictionary<string, string> ToDictionary(this Properties properties)
    {
      return properties == null
        ? new Dictionary<string, string>()
        : properties.Property.ToDictionary(p => p.Name, p => p.Value, StringComparer.OrdinalIgnoreCase);
    }

    public static Dictionary<string, string> GetProperties(this Object obj, Dictionary<string, Objecttype> objecttypesByName)
    {
      var properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

      Objecttype objecttype;

      if (!string.IsNullOrEmpty(obj.Type)
        && objecttypesByName.TryGetValue(obj.Type, out objecttype))
      {
        foreach (var property in objecttype.Properties)
        {
          properties[property.Name] = property.Default;
        }
      }

      if (obj.Properties != null)
      {
        foreach (var property in obj.Properties.Property)
        {
          properties[property.Name] = property.Value;
        }
      }

      return properties;
    }

    public static IEnumerable<Objectgroup> ForEachObjectGroupWithProperties(
      this Map map,
      Property[] properties)
    {
      return map
        .Objectgroup
        .Where(og => properties.All(property => og.HasProperty(property.Name, property.Value)));
    }

    public static IEnumerable<Objectgroup> ForEachObjectGroupWithPropertyName(
      this Map map,
      string propertyName)
    {
      return map
        .Objectgroup
        .Where(og => og.HasProperty(propertyName));
    }

    public static IEnumerable<Objectgroup> ForEachObjectGroupWithProperty(
      this Map map,
      string propertyName,
      string propertyValue)
    {
      return map
        .Objectgroup
        .Where(og => og.HasProperty(propertyName, propertyValue));
    }

    public static IEnumerable<Object> ForEachObjectWithProperty(
      this Map map,
      Property[] propertyFilters,
      string propertyName,
      Dictionary<string, Objecttype> objecttypesByName)
    {
      return map
        .ForEachObjectGroupWithProperties(propertyFilters)
        .SelectMany(og => og
          .Object
          .Where(o => o.HasProperty(propertyName, objecttypesByName)));
    }

    public static IEnumerable<Object> ForEachObjectWithProperty(
      this Map map,
      string propertyName,
      Dictionary<string, Objecttype> objecttypesByName)
    {
      return map
        .Objectgroup
        .SelectMany(og => og
          .Object
          .Where(o => o.HasProperty(propertyName, objecttypesByName)));
    }

    public static bool HasProperty(this Object obj, string propertyName, Dictionary<string, Objecttype> objecttypesByName)
    {
      return GetProperties(obj, objecttypesByName)
        .ContainsKey(propertyName);
    }

    public static bool HasProperty(this Object obj, string propertyName, string propertyValue, Dictionary<string, Objecttype> objecttypesByName)
    {
      var properties = GetProperties(obj, objecttypesByName);

      string value;

      return properties.TryGetValue(propertyName, out value)
        && string.Equals(propertyValue, value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasProperty(this Objectgroup obj, string propertyName)
    {
      return obj.Properties.ToDictionary().ContainsKey(propertyName);
    }

    public static bool HasProperty(this Objectgroup obj, string propertyName, string propertyValue)
    {
      string value;

      return obj.Properties.ToDictionary().TryGetValue(propertyName, out value)
        && string.Equals(propertyValue, value, StringComparison.OrdinalIgnoreCase);
    }

    public static Object[] GetTypesOrThrow(this Objectgroup objectgroup, string typeName)
    {
      var items = objectgroup.GetTypes(typeName).ToArray();

      if (items == null || !items.Any())
      {
        string errorMessage = "Unable to load any objects of type '" + typeName + "' for object '" + objectgroup.Name + "'";

        Debug.LogError(errorMessage);

        throw new Exception(errorMessage);
      }

      return items;
    }

    public static IEnumerable<Object> GetTypes(this Objectgroup objectgroup, string typeName)
    {
      return objectgroup
        .Object
        .Where(o => string.Equals(o.Type, typeName, StringComparison.InvariantCultureIgnoreCase));
    }

    public static Object GetTypeOrThrow(this Objectgroup objectgroup, string typeName)
    {
      var obj = objectgroup.GetType(typeName);

      if (obj == null)
      {
        string errorMessage = "Unable to load object of type '" + typeName + "' for object '" + objectgroup.Name + "'";

        Debug.LogError(errorMessage);

        throw new Exception(errorMessage);
      }

      return obj;
    }

    public static Object GetType(this Objectgroup objectgroup, string typeName)
    {
      return objectgroup
        .Object
        .Where(o => string.Equals(o.Type, typeName, StringComparison.InvariantCultureIgnoreCase))
        .FirstOrDefault();
    }

    public static Bounds GetBounds(this Object obj)
    {
      if (obj.IsImage())
      {
        return new Bounds(
          new Vector2(obj.X + obj.Width / 2, -obj.Y),
          new Vector2(obj.Width, obj.Height));
      }

      return new Bounds(
          new Vector2(obj.X + obj.Width / 2, -(obj.Y + obj.Height / 2)),
          new Vector2(obj.Width, obj.Height));
    }

    public static bool IsImage(this Object obj)
    {
      return obj.Gid.HasValue;
    }

    public static bool IsCollider(this Object obj)
    {
      return !obj.Gid.HasValue;
    }

    public static IEnumerable<Layer> ForEachLayerWithProperties(
      this Map map,
      Property[] properties)
    {
      return map
        .Layers
        .Where(
          layer => layer.Properties != null
          && properties.All(property => layer.Properties.Property.Any(
            p => string.Equals(p.Name.Trim(), property.Name, StringComparison.OrdinalIgnoreCase)
              && string.Equals(p.Value.Trim(), property.Value, StringComparison.OrdinalIgnoreCase))));
    }

    public static IEnumerable<Layer> ForEachLayerWithProperty(this Map map, string propertyName, string propertyValue)
    {
      return map
        .Layers
        .Where(
          layer => layer.Properties != null
          && layer.Properties.Property.Any(
            p => string.Equals(p.Name.Trim(), propertyName, StringComparison.OrdinalIgnoreCase)
              && string.Equals(p.Value.Trim(), propertyValue, StringComparison.OrdinalIgnoreCase)));
    }

    public static IEnumerable<Layer> ForEachLayerWithPropertyName(this Map map, string propertyName)
    {
      return map
        .Layers
        .Where(layer => layer.HasProperty(propertyName));
    }

    public static bool HasProperty(this Layer layer, string propertyName, string propertyValue)
    {
      string value;

      return layer.Properties.ToDictionary().TryGetValue(propertyName, out value)
        && string.Equals(propertyValue, value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasProperty(this Layer layer, string propertyName)
    {
      return layer.Properties != null
        && layer.Properties.Property.Any(
          p => string.Equals(p.Name.Trim(), propertyName, StringComparison.OrdinalIgnoreCase));
    }

    public static bool TryGetProperty(this Layer layer, string propertyName, out int value)
    {
      string valueText;

      if (layer.TryGetProperty(propertyName, out valueText))
      {
        value = int.Parse(valueText);

        return true;
      }

      value = 0;

      return false;
    }

    public static bool TryGetProperty(this Layer layer, string propertyName, out string value)
    {
      var property = layer
        .Properties
        .Property
        .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

      if (property != null)
      {
        value = property.Value;

        return true;
      }

      value = null;
      return false;
    }

    private static string GetPropertyValue(Properties properties, string propertyName)
    {
      return properties
        .Property
        .First(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
        .Value;
    }

    public static string GetPropertyValue(this Objectgroup objectgroup, string propertyName)
    {
      return GetPropertyValue(
        objectgroup.Properties,
        propertyName);
    }

    public static string GetPropertyValue(this Layer layer, string propertyName)
    {
      return GetPropertyValue(
        layer.Properties,
        propertyName);
    }

    public static void Execute(this IEnumerable<Layer> layers, Action<Layer> action)
    {
      foreach (var layer in layers)
      {
        action(layer);
      }
    }

    public static IEnumerable<T> Get<T>(this IEnumerable<Layer> layers, Func<Layer, T> func)
    {
      foreach (var layer in layers)
      {
        yield return func(layer);
      }
    }

    public static IEnumerable<T> Get<T>(this IEnumerable<Object> objects, Func<Object, T> func)
    {
      foreach (var obj in objects)
      {
        yield return func(obj);
      }
    }

    public static IEnumerable<T> Get<T>(this IEnumerable<Objectgroup> objectgroups, Func<Objectgroup, T> func)
    {
      foreach (var obj in objectgroups)
      {
        yield return func(obj);
      }
    }
  }
}
