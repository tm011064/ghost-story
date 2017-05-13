using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "objecttype")]
  public class ObjectType
  {
    [XmlElement(ElementName = "property")]
    public List<ObjectProperty> Properties { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "color")]
    public string Color { get; set; }
  }
}
