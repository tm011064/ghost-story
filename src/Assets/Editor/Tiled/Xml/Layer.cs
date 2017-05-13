using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "layer")]
  public class Layer
  {
    [XmlElement(ElementName = "data")]
    public Data Data { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public string Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public string Height { get; set; }

    [XmlElement(ElementName = "properties")]
    public PropertyGroup PropertyGroup { get; set; }
  }
}
