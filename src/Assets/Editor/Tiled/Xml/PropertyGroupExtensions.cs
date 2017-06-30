using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Editor.Tiled.Xml
{
  public static class PropertyGroupExtensions
  {
    public static Dictionary<string, string> ToDictionary(this PropertyGroup group)
    {
      return group == null
        ? new Dictionary<string, string>()
        : group.Properties.ToDictionary(p => p.Name, p => p.Value, StringComparer.OrdinalIgnoreCase);
    }

    public static string GetPropertyValue(this PropertyGroup group, string propertyName)
    {
      var values = group
        .Properties
        .Where(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
        .ToArray();

      if (!values.Any())
      {
        throw new KeyNotFoundException("Property with name '" + propertyName + "' not found");
      }

      if (values.Count() > 1)
      {
        throw new InvalidOperationException("Multiple properties with name '" + propertyName + "' found");
      }

      return values.Single().Value;
    }

    public static int GetPropertyValueAsInt32(this PropertyGroup group, string propertyName)
    {
      var value = group.GetPropertyValue(propertyName);

      int parsed;
      if (int.TryParse(value, out parsed))
      {
        return parsed;
      }

      throw new FormatException("Unable to parse value '" + value + "' from property '" + propertyName + "'");
    }
  }
}
