using System.Collections.Generic;
using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  public class GhostStoryTimerPlatformFactory : TimerPlatformFactory
  {
    public GhostStoryTimerPlatformFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup)
      : base(root, map, prefabLookup)
    {
    }

    protected override void OnGameObjectCreated(AbstractTiledLayerConfig layerConfig, GameObject gameObject)
    {
      gameObject.AddLevelConfigComponent(layerConfig);
    }
  }
}
