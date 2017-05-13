using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "objectgroup")]
  public class ObjectGroup
  {
    [XmlElement(ElementName = "object")]
    public List<TiledObject> Objects { get; set; }

    [XmlAttribute(AttributeName = "color")]
    public string Color { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "properties")]
    public PropertyGroup PropertyGroup { get; set; }
  }
}
