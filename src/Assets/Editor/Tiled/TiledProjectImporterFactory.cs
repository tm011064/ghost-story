using System.Collections.Generic;
using System.IO;
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

      var group = new ObjectTypeGroup { ObjectTypes = new List<ObjectType>() };

      using (var reader = new StreamReader(mapFilePath))
      {
        map = (Map)mapSerializer.Deserialize(reader);
      }

      if (!string.IsNullOrEmpty(objectTypesFilePath))
      {
        var objectTypesSerializer = new XmlSerializer(typeof(ObjectTypeGroup));

        using (var reader = new StreamReader(objectTypesFilePath))
        {
          group = (ObjectTypeGroup)objectTypesSerializer.Deserialize(reader);
        }
      }

      return new TiledProjectImporter(map, group);
    }
  }
}
