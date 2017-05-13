using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "polyline")]
  public class PolyLine
  {
    [XmlAttribute(AttributeName = "points")]
    public string Points { get; set; }
  }
}
