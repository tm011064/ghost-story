using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Assets.Editor.Tiled.Xml;

namespace Assets.Editor.Tiled
{
  public static class TiledProjectImporterFactory
  {
    public static TiledProjectImporter CreateFromFile(string mapFilePath, string objectTypesFilePath = null)
    {
      var mapSerializer = new XmlSerializer(typeof(Map));

      Map map;

      var objectTypeGroup = new ObjectTypeGroup { ObjectTypes = new List<ObjectType>() };

      using (var reader = new StreamReader(mapFilePath))
      {
        map = (Map)mapSerializer.Deserialize(reader);
      }

      Logger.UnityDebugLog("Deserialized map");

      PropagateGroupProperties(map);

      if (!string.IsNullOrEmpty(objectTypesFilePath))
      {
        var objectTypesSerializer = new XmlSerializer(typeof(ObjectTypeGroup));

        using (var reader = new StreamReader(objectTypesFilePath))
        {
          objectTypeGroup = (ObjectTypeGroup)objectTypesSerializer.Deserialize(reader);
        }

        Logger.UnityDebugLog("Deserialized object type group");

        PropagateObjectTypeGroupProperties(map, objectTypeGroup);
      }
      else
      {
        Logger.UnityDebugLog("Object type group deserialization skipped, no file provided");
      }

      return new TiledProjectImporter(map);
    }

    private static void PropagateObjectTypeGroupProperties(Map map, ObjectTypeGroup objectTypeGroup)
    {
      Logger.UnityDebugLog("Propagating object type group properties");

      var objectTypesByName = objectTypeGroup
        .ObjectTypes
        .ToDictionary(ot => ot.Name, ot => ot, StringComparer.InvariantCultureIgnoreCase);

      ObjectType objectType;
      foreach (var objectGroup in map.AllObjectGroups())
      {
        foreach (var tiledObject in objectGroup.Objects)
        {
          if (tiledObject.PropertyGroup == null)
          {
            tiledObject.PropertyGroup = new PropertyGroup { Properties = new List<Property>() };
          }

          if (!string.IsNullOrEmpty(tiledObject.Type)
            && objectTypesByName.TryGetValue(tiledObject.Type, out objectType))
          {
            foreach (var property in objectType.Properties)
            {
              if (!tiledObject.HasProperty(property.Name))
              {
                tiledObject.PropertyGroup.Properties.Add(new Property
                {
                  Name = property.Name,
                  Value = property.Default
                });
              }
            }
          }
        }
      }
    }

    private static void PropagateGroupProperties(Map map)
    {
      Logger.UnityDebugLog("Propagating group properties");

      foreach (var group in map.AllGroupsDepthFirst())
      {
        AssignProperties(group);
      }
    }

    private static void AssignPropertyIfNotExists(IHasPropertyGroup obj, Property property)
    {
      obj.EnsurePropertyGroupExists();

      if (!obj.HasProperty(property.Name))
      {
        obj.PropertyGroup.Properties.Add(property);
      }
    }

    private static void AssignProperties(Group group)
    {
      group.EnsurePropertyGroupExists();

      var propertyGroupContainers = group.ObjectGroups.Cast<IHasPropertyGroup>()
        .Concat(group.Layers.Cast<IHasPropertyGroup>())
        .Concat(group.Groups.Cast<IHasPropertyGroup>())
        .ToArray();

      foreach (var property in group.PropertyGroup.Properties)
      {
        foreach (var propertyGroupContainer in propertyGroupContainers)
        {
          AssignPropertyIfNotExists(propertyGroupContainer, property);
        }
      }
    }
  }
}
