using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled
{
  public static class TiledProjectImporterFactory
  {
    public static TiledProjectImporter CreateFromFile(string mapFilePath, string objectTypesFilePath = null)
    {
      var mapSerializer = new XmlSerializer(typeof(Map));

      Map map;

      Objecttypes objecttypes = new Objecttypes { Objecttype = new List<Objecttype>() };

      using (var reader = new StreamReader(mapFilePath))
      {
        map = (Map)mapSerializer.Deserialize(reader);
      }

      if (!string.IsNullOrEmpty(objectTypesFilePath))
      {
        var objecttypesSerializer = new XmlSerializer(typeof(Objecttypes));

        using (var reader = new StreamReader(objectTypesFilePath))
        {
          objecttypes = (Objecttypes)objecttypesSerializer.Deserialize(reader);
        }
      }

      return new TiledProjectImporter(map, objecttypes);
    }
  }
}
