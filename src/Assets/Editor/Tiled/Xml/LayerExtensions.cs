using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Editor.Tiled.Xml
{
  public static class LayerExtensions
  {
    public static bool HasProperty(this Layer layer, string propertyName, string propertyValue)
    {
      string value;

      return layer.PropertyGroup.ToDictionary().TryGetValue(propertyName, out value)
        && string.Equals(propertyValue, value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasProperty(this Layer layer, string propertyName)
    {
      return layer.PropertyGroup != null
        && layer.PropertyGroup.Properties.Any(
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
        .PropertyGroup
        .Properties
        .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

      if (property != null)
      {
        value = property.Value;

        return true;
      }

      value = null;
      return false;
    }

    public static IEnumerable<string> GetCommands(this Layer layer)
    {
      if (!layer.HasProperty("Commands"))
      {
        return Enumerable.Empty<string>();
      }

      return layer.GetPropertyValue("Commands")
        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());
    }

    public static string GetPropertyValue(this Layer layer, string propertyName)
    {
      return layer.PropertyGroup.GetPropertyValue(propertyName);
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
  }
}
