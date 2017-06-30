using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "object")]
  public class TiledObject : IHasType, IHasPropertyGroup
  {
    [XmlAttribute(AttributeName = "id")]
    public string Id { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlAttribute(AttributeName = "x")]
    public int X { get; set; }

    [XmlAttribute(AttributeName = "y")]
    public int Y { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }

    [XmlAttribute(AttributeName = "gid")]
    public long? Gid { get; set; }

    [XmlElement(ElementName = "properties")]
    public PropertyGroup PropertyGroup { get; set; }

    [XmlElement(ElementName = "polyline")]
    public PolyLine PolyLine { get; set; }

    [XmlElement(ElementName = "text")]
    public Text Text { get; set; }
  }
}
