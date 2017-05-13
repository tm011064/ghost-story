using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Editor.Tiled.Xml
{
  public static class MapExxtensions
  {
    public static IEnumerable<ObjectGroup> ForEachObjectGroupWithProperties(
      this Map map,
      Property[] properties)
    {
      return map
        .ObjectGroups
        .Where(og => properties.All(property => og.HasProperty(property.Name, property.Value)));
    }

    public static IEnumerable<ObjectGroup> ForEachObjectGroupWithPropertyName(
      this Map map,
      string propertyName)
    {
      return map
        .ObjectGroups
        .Where(og => og.HasProperty(propertyName));
    }

    public static IEnumerable<ObjectGroup> ForEachObjectGroupWithProperty(
      this Map map,
      string propertyName,
      string propertyValue)
    {
      return map
        .ObjectGroups
        .Where(og => og.HasProperty(propertyName, propertyValue));
    }

    public static IEnumerable<TiledObject> ForEachObjectWithProperty(
      this Map map,
      Property[] propertyFilters,
      string propertyName,
      Dictionary<string, ObjectType> objectTypesByName)
    {
      return map
        .ForEachObjectGroupWithProperties(propertyFilters)
        .SelectMany(og => og
          .Objects
          .Where(o => o.HasProperty(propertyName, objectTypesByName)));
    }

    public static IEnumerable<TiledObject> ForEachObjectWithProperty(
      this Map map,
      string propertyName,
      Dictionary<string, ObjectType> objectTypesByName)
    {
      return map
        .ObjectGroups
        .SelectMany(og => og
          .Objects
          .Where(o => o.HasProperty(propertyName, objectTypesByName)));
    }

    public static IEnumerable<Layer> ForEachLayerWithProperties(
      this Map map,
      Property[] properties)
    {
      return map
        .Layers
        .Where(
          layer => layer.PropertyGroup != null
          && properties.All(property => layer.PropertyGroup.Properties.Any(
            p => string.Equals(p.Name.Trim(), property.Name, StringComparison.OrdinalIgnoreCase)
              && string.Equals(p.Value.Trim(), property.Value, StringComparison.OrdinalIgnoreCase))));
    }

    public static IEnumerable<Layer> ForEachLayerWithProperty(this Map map, string propertyName, string propertyValue)
    {
      return map
        .Layers
        .Where(
          layer => layer.PropertyGroup != null
          && layer.PropertyGroup.Properties.Any(
            p => string.Equals(p.Name.Trim(), propertyName, StringComparison.OrdinalIgnoreCase)
              && string.Equals(p.Value.Trim(), propertyValue, StringComparison.OrdinalIgnoreCase)));
    }

    public static IEnumerable<Layer> ForEachLayerWithPropertyName(this Map map, string propertyName)
    {
      return map
        .Layers
        .Where(layer => layer.HasProperty(propertyName));
    }
  }
}
