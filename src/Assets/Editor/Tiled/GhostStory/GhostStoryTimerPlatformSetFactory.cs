using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  public class GhostStoryTimerPlatformSetFactory : TimerPlatformSetFactory
  {
    public GhostStoryTimerPlatformSetFactory(
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
