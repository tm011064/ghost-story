using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.Xml
{
  public static class ObjectGroupExtensions
  {
    public static TiledObject[] GetTiledObjectsOrThrow(this ObjectGroup group, string typeName)
    {
      var items = group.GetTiledObjects(typeName).ToArray();

      if (items == null || !items.Any())
      {
        string errorMessage = "Unable to load any objects of type '" + typeName + "' for object '" + group.Name + "'";

        Debug.LogError(errorMessage);

        throw new Exception(errorMessage);
      }

      return items;
    }

    public static IEnumerable<TiledObject> GetTiledObjects(this ObjectGroup group, string typeName)
    {
      return group
        .Objects
        .Where(o => string.Equals(o.Type, typeName, StringComparison.InvariantCultureIgnoreCase));
    }

    public static TiledObject GetTiledObjectOrThrow(this ObjectGroup group, string typeName)
    {
      var obj = group.GetTiledObject(typeName);

      if (obj == null)
      {
        string errorMessage = "Unable to load object of type '" + typeName + "' for object '" + group.Name + "'";

        Debug.LogError(errorMessage);

        throw new Exception(errorMessage);
      }

      return obj;
    }

    public static TiledObject GetTiledObject(this ObjectGroup group, string typeName)
    {
      return group
        .Objects
        .Where(o => string.Equals(o.Type, typeName, StringComparison.InvariantCultureIgnoreCase))
        .FirstOrDefault();
    }    
  }
}
