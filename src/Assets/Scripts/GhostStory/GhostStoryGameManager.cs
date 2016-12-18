using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GhostStory.Behaviours;
using UnityEngine;

public class GhostStoryGameManager : GameManager
{
  protected override void OnAwake()
  {
    var gameObjectsByLayerUniverseKey = GameObject
      .FindObjectsOfType<LevelObjectConfig>()
      .Select(config => new { Keys = CreateKeys(config).ToArray(), GameObject = config.gameObject })
      .SelectMany(c => c.Keys.Select(k => new { Key = k, GameObject = c.GameObject }))
      .ToLookup(c => c.Key, c => c.GameObject);

    GhostStoryGameContext.Instance = new GhostStoryGameContext(gameObjectsByLayerUniverseKey);
  }

  private IEnumerable<LayerUniverseKey> CreateKeys(LevelObjectConfig levelObjectConfig)
  {
    if (levelObjectConfig.Universe == Universe.Global)
    {
      yield return new LayerUniverseKey { Layer = levelObjectConfig.Layer, Universe = Universe.AlternateWorld };
      yield return new LayerUniverseKey { Layer = levelObjectConfig.Layer, Universe = Universe.RealWorld };

      yield break;
    }

    yield return new LayerUniverseKey { Layer = levelObjectConfig.Layer, Universe = levelObjectConfig.Universe };
  }

  protected override void OnStart()
  {
    GhostStoryGameContext.Instance.DisableAndHideAllObjects();
    GhostStoryGameContext.Instance.SwitchLayer(LevelLayer.House);
    GhostStoryGameContext.Instance.SwitchToRealWorld();
  }
}
