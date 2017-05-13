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
      return group
        .Properties
        .First(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
        .Value;
    }
  }
}
