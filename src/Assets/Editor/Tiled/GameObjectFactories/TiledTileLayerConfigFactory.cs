using System.Linq;
using Assets.Editor.Tiled.Xml;

namespace Assets.Editor.Tiled.GameObjectFactories
{
  public static class TiledTileLayerConfigFactory
  {
    public static TiledTileLayerConfig Create(Layer layer)
    {
      return new TiledTileLayerConfig
      {
        TiledLayer = layer,
        Type = layer.GetPropertyValue("Type"),
        Universe = layer.GetPropertyValue("Universe"),
        Commands = layer.GetCommands().ToArray()
      };
    }
  }
}
