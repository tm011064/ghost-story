using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "objecttypes")]
  public class ObjectTypeGroup
  {
    [XmlElement(ElementName = "objecttype")]
    public List<ObjectType> ObjectTypes { get; set; }
  }
}
