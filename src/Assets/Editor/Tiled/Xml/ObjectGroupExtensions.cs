using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.Xml
{
  public static class ObjectGroupExtensions
  {
    public static bool HasProperty(this ObjectGroup group, string propertyName)
    {
      return group.PropertyGroup.ToDictionary().ContainsKey(propertyName);
    }

    public static bool HasProperty(this ObjectGroup group, string propertyName, string propertyValue)
    {
      string value;

      return group.PropertyGroup.ToDictionary().TryGetValue(propertyName, out value)
        && string.Equals(propertyValue, value, StringComparison.OrdinalIgnoreCase);
    }

    public static TiledObject[] GetTypesOrThrow(this ObjectGroup group, string typeName)
    {
      var items = group.GetTypes(typeName).ToArray();

      if (items == null || !items.Any())
      {
        string errorMessage = "Unable to load any objects of type '" + typeName + "' for object '" + group.Name + "'";

        Debug.LogError(errorMessage);

        throw new Exception(errorMessage);
      }

      return items;
    }

    public static IEnumerable<TiledObject> GetTypes(this ObjectGroup group, string typeName)
    {
      return group
        .Objects
        .Where(o => string.Equals(o.Type, typeName, StringComparison.InvariantCultureIgnoreCase));
    }

    public static TiledObject GetTypeOrThrow(this ObjectGroup group, string typeName)
    {
      var obj = group.GetType(typeName);

      if (obj == null)
      {
        string errorMessage = "Unable to load object of type '" + typeName + "' for object '" + group.Name + "'";

        Debug.LogError(errorMessage);

        throw new Exception(errorMessage);
      }

      return obj;
    }

    public static TiledObject GetType(this ObjectGroup group, string typeName)
    {
      return group
        .Objects
        .Where(o => string.Equals(o.Type, typeName, StringComparison.InvariantCultureIgnoreCase))
        .FirstOrDefault();
    }

    public static IEnumerable<string> GetCommands(this ObjectGroup group)
    {
      if (!group.HasProperty("Commands"))
      {
        return Enumerable.Empty<string>();
      }

      return group.GetPropertyValue("Commands")
        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());
    }

    public static string GetPropertyValue(this ObjectGroup group, string propertyName)
    {
      return group.PropertyGroup.GetPropertyValue(propertyName);
    }

    public static IEnumerable<T> Get<T>(this IEnumerable<ObjectGroup> groups, Func<ObjectGroup, T> func)
    {
      foreach (var obj in groups)
      {
        yield return func(obj);
      }
    }
  }
}
