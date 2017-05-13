using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "properties")]
  public class PropertyGroup
  {
    [XmlElement(ElementName = "property")]
    public List<Property> Properties { get; set; }
  }
}
