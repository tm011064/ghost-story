using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  public class Text
  {
    [XmlAttribute(AttributeName = "wrap")]
    public string Wrap { get; set; }

    [XmlAttribute(AttributeName = "color")]
    public string Color { get; set; }

    [XmlText]
    public string Content { get; set; }
  }
}
