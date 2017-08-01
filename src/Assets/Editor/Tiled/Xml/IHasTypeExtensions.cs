using System;

namespace Assets.Editor.Tiled.Xml
{
  public static class IHasTypeExtensions
  {
    public static bool IsType(this IHasType self, string typeName)
    {
      return string.Equals(self.Type, typeName, StringComparison.OrdinalIgnoreCase);
    }
  }
}
