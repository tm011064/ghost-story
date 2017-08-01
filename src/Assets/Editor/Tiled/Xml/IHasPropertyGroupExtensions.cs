using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Editor.Tiled.Xml
{
  public static class IHasPropertyGroupExtensions
  {
    public static string GetPropertyValue(this IHasPropertyGroup self, string propertyName)
    {
      return self.PropertyGroup.GetPropertyValue(propertyName);
    }

    public static bool HasProperty(this IHasPropertyGroup self, string propertyName, string propertyValue)
    {
      string value;

      return self.PropertyGroup.ToDictionary().TryGetValue(propertyName, out value)
        && string.Equals(propertyValue, value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasProperty(this IHasPropertyGroup self, string propertyName)
    {
      return self.PropertyGroup != null
        && self.PropertyGroup.Properties.Any(
          p => string.Equals(p.Name.Trim(), propertyName, StringComparison.OrdinalIgnoreCase));
    }

    public static int TryGetPropertyAsInt32(this IHasPropertyGroup group, string propertyName, int defaultValue = 0)
    {
      int value;
      return int.TryParse(group.TryGetProperty(propertyName), out value)
        ? value
        : defaultValue;
    }

    public static string TryGetProperty(this IHasPropertyGroup self, string propertyName, string defaultValue = null)
    {
      var property = self
        .PropertyGroup
        .Properties
        .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

      return property != null
        ? property.Value
        : defaultValue;
    }

    public static IEnumerable<string> GetCommands(this IHasPropertyGroup self)
    {
      if (!self.HasProperty("Commands"))
      {
        return Enumerable.Empty<string>();
      }

      return self.GetPropertyValue("Commands")
        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());
    }

    public static void EnsurePropertyGroupExists(this IHasPropertyGroup self)
    {
      if (self.PropertyGroup == null)
      {
        self.PropertyGroup = new PropertyGroup { Properties = new List<Property>() };
      }
    }
  }
}
