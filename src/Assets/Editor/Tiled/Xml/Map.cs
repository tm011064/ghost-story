using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled.Xml
{
  [XmlRoot(ElementName = "map")]
  public class Map
  {
    [XmlElement(ElementName = "tileset")]
    public List<TileSet> TileSets { get; set; }

    [XmlElement(ElementName = "group")]
    public List<Group> Groups { get; set; }

    [XmlElement(ElementName = "layer")]
    public List<Layer> Layers { get; set; }

    [XmlElement(ElementName = "objectgroup")]
    public List<ObjectGroup> ObjectGroups { get; set; }

    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }

    [XmlAttribute(AttributeName = "orientation")]
    public string Orientation { get; set; }

    [XmlAttribute(AttributeName = "renderorder")]
    public string RenderOrder { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }

    [XmlAttribute(AttributeName = "tilewidth")]
    public int TileWidth { get; set; }

    [XmlAttribute(AttributeName = "tileheight")]
    public int TileHeight { get; set; }

    [XmlAttribute(AttributeName = "nextobjectid")]
    public string NextObjectId { get; set; }
  }
}
