using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "image")]
  public class Image
  {
    [XmlAttribute(AttributeName = "source")]
    public string Source { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public string Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public string Height { get; set; }
  }
}
