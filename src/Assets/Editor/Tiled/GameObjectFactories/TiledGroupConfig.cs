using Assets.Editor.Tiled.Xml;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public class TiledGroupConfig : AbstractTiledLayerConfig
  {
    public Group Group;

    public TiledObjectLayerConfig[] ObjectLayerConfigs;

    public TiledGroupConfig[] GroupConfigs;

    public TiledTileLayerConfig[] TileLayerConfigs;
  }
}
