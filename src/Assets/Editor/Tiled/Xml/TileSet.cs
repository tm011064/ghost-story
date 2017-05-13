using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "tileset")]
  public class TileSet
  {
    [XmlElement(ElementName = "image")]
    public Image Image { get; set; }

    [XmlAttribute(AttributeName = "firstgid")]
    public string FirstGid { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "tilewidth")]
    public string TileWidth { get; set; }

    [XmlAttribute(AttributeName = "tileheight")]
    public string TileHeight { get; set; }

    [XmlAttribute(AttributeName = "tilecount")]
    public string TileCount { get; set; }

    [XmlAttribute(AttributeName = "columns")]
    public string Columns { get; set; }
  }
}
