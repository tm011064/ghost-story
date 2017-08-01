using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Editor.Tiled.Xml
{
  public static class MapExtensions
  {
    private static IEnumerable<Group> TraverseGroupsDepthFirst(Group group, Func<Group, bool> filterPredicate)
    {
      yield return group;

      if (group.Groups == null)
      {
        yield break;
      }

      foreach (var subGroup in group.Groups.Where(filterPredicate).SelectMany(g => TraverseGroupsDepthFirst(g, filterPredicate)))
      {
        yield return subGroup;
      }
    }

    public static IEnumerable<Group> AllGroupsDepthFirst(this Map map, Func<Group, bool> filterPredicate = null)
    {
      if (map.Groups == null)
      {
        return Enumerable.Empty<Group>();
      }

      filterPredicate = filterPredicate ?? (_ => true);

      return map
        .Groups
        .Where(filterPredicate)
        .SelectMany(g => TraverseGroupsDepthFirst(g, filterPredicate));
    }

    public static IEnumerable<ObjectGroup> AllObjectGroups(this Map map)
    {
      var grouped = map.AllGroupsDepthFirst().SelectMany(g => g.ObjectGroups);

      return map.ObjectGroups.Concat(grouped);
    }

    public static IEnumerable<Layer> AllLayers(this Map map)
    {
      var grouped = map.AllGroupsDepthFirst().SelectMany(g => g.Layers);

      return map.Layers.Concat(grouped);
    }

    public static IEnumerable<ObjectGroup> ForEachObjectGroupWithProperties(
      this Map map,
      Property[] properties)
    {
      return map
        .AllObjectGroups()
        .Where(og => properties.All(property => og.HasProperty(property.Name, property.Value)));
    }

    public static IEnumerable<ObjectGroup> ForEachObjectGroupWithPropertyName(
      this Map map,
      string propertyName)
    {
      return map
        .AllObjectGroups()
        .Where(og => og.HasProperty(propertyName));
    }

    public static IEnumerable<ObjectGroup> ForEachObjectGroupWithProperty(
      this Map map,
      string propertyName,
      string propertyValue)
    {
      return map
        .AllObjectGroups()
        .Where(og => og.HasProperty(propertyName, propertyValue));
    }

    public static IEnumerable<TiledObject> ForEachObjectWithProperty(
      this Map map,
      Property[] propertyFilters,
      string propertyName)
    {
      return map
        .ForEachObjectGroupWithProperties(propertyFilters)
        .SelectMany(og => og
          .Objects
          .Where(o => o.HasProperty(propertyName)));
    }

    public static IEnumerable<TiledObject> ForEachObjectWithProperty(
      this Map map,
      string propertyName)
    {
      return map
        .AllObjectGroups()
        .SelectMany(og => og
          .Objects
          .Where(o => o.HasProperty(propertyName)));
    }

    public static IEnumerable<Layer> ForEachLayerWithProperties(
      this Map map,
      Property[] properties)
    {
      return map
        .AllLayers()
        .Where(
          layer => layer.PropertyGroup != null
          && properties.All(property => layer.PropertyGroup.Properties.Any(
            p => string.Equals(p.Name.Trim(), property.Name, StringComparison.OrdinalIgnoreCase)
              && string.Equals(p.Value.Trim(), property.Value, StringComparison.OrdinalIgnoreCase))));
    }

    public static IEnumerable<Layer> ForEachLayerWithProperty(this Map map, string propertyName, string propertyValue)
    {
      return map
        .AllLayers()
        .Where(
          layer => layer.PropertyGroup != null
          && layer.PropertyGroup.Properties.Any(
            p => string.Equals(p.Name.Trim(), propertyName, StringComparison.OrdinalIgnoreCase)
              && string.Equals(p.Value.Trim(), propertyValue, StringComparison.OrdinalIgnoreCase)));
    }

    public static IEnumerable<Layer> ForEachLayerWithPropertyName(this Map map, string propertyName)
    {
      return map
        .AllLayers()
        .Where(layer => layer.HasProperty(propertyName));
    }
  }
}
