namespace Assets.Editor.Tiled.GameObjectFactories
{
  public static class TiledTileLayerConfigFactory
  {
    public static TiledTileLayerConfig Create(Layer layer)
    {
      return new TiledTileLayerConfig
      {
        TiledLayer = layer,
        Layer = layer.GetPropertyValue("Layer"),
        Type = layer.GetPropertyValue("Type"),
        Universe = layer.GetPropertyValue("Universe")
      };
    }
  }
}
