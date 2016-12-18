using System;
using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Scripts.GhostStory.Behaviours;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  public static class GameObjectExtensions
  {
    public static void AddLevelConfigComponent(this GameObject gameObject, AbstractTiledLayerConfig layerConfig)
    {
      var config = gameObject.AddComponent<LevelObjectConfig>();

      config.Universe = (Universe)Enum.Parse(typeof(Universe), layerConfig.Universe);
      config.Layer = (LevelLayer)Enum.Parse(typeof(LevelLayer), layerConfig.Layer);
    }
  }
}
