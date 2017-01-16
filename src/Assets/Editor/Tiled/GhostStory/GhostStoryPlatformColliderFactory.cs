using System.Collections.Generic;
using Assets.Editor.Tiled.GameObjectFactories;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  public class GhostStoryPlatformColliderFactory : PlatformColliderFactory
  {
    public GhostStoryPlatformColliderFactory(
      GameObject parent,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, Objecttype> objecttypesByName)
      : base(parent, map, prefabLookup, objecttypesByName)
    {
    }

    protected override void OnGameObjectCreated(AbstractTiledLayerConfig layerConfig, GameObject gameObject)
    {
      gameObject.AddLevelConfigComponent(layerConfig);
    }
  }
}
