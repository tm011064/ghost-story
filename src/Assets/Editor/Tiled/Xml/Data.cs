using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "data")]
  public class Data
  {
    [XmlAttribute(AttributeName = "encoding")]
    public string Encoding { get; set; }

    [XmlText]
    public string Text { get; set; }
  }
}
