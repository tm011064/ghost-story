using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "group")]
  public class Group : IHasPropertyGroup
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "group")]
    public List<Group> Groups { get; set; }

    [XmlElement(ElementName = "layer")]
    public List<Layer> Layers { get; set; }

    [XmlElement(ElementName = "objectgroup")]
    public List<ObjectGroup> ObjectGroups { get; set; }

    [XmlElement(ElementName = "properties")]
    public PropertyGroup PropertyGroup { get; set; }
  }
}
