using System.Linq;
using Assets.Scripts.GhostStory;
using UnityEngine;

public class GhostStoryGameContext
{
  public static GhostStoryGameContext Instance;

  private ILookup<LayerUniverseKey, GameObject> _gameObjectsByLayerUniverseKey;

  private LayerUniverseKey _activeUniverse;

  public GhostStoryGameContext(
    ILookup<LayerUniverseKey, GameObject> gameObjectsByLayerUniverseKey)
  {
    _gameObjectsByLayerUniverseKey = gameObjectsByLayerUniverseKey;
  }

  public void DisableAndHideAllObjects()
  {
    foreach (var lookupGrouping in _gameObjectsByLayerUniverseKey)
    {
      foreach (var gameObject in lookupGrouping)
      {
        gameObject.DisableAndHide();
      }
    }
  }

  public bool IsRealWorldActivated()
  {
    return _activeUniverse.Universe == Universe.RealWorld;
  }

  public bool IsAlternateWorldActivated()
  {
    return _activeUniverse.Universe == Universe.AlternateWorld;
  }

  private void DisableCurrentGameObjects()
  {
    foreach (var gameObject in _gameObjectsByLayerUniverseKey[_activeUniverse])
    {
      gameObject.DisableAndHide();
    }
  }

  private void EnableCurrentGameObjects()
  {
    foreach (var gameObject in _gameObjectsByLayerUniverseKey[_activeUniverse])
    {
      gameObject.EnableAndShow();
    }
  }

  private void SwitchUniverse(Universe universe)
  {
    DisableCurrentGameObjects();

    _activeUniverse = new LayerUniverseKey { Layer = _activeUniverse.Layer, Universe = universe };

    EnableCurrentGameObjects();
  }

  public void SwitchToRealWorld()
  {
    SwitchUniverse(Universe.RealWorld);
  }

  public void SwitchToAlternateWorld()
  {
    SwitchUniverse(Universe.AlternateWorld);
  }

  public void SwitchLayer(LevelLayer layer)
  {
    DisableCurrentGameObjects();

    _activeUniverse = new LayerUniverseKey { Layer = layer, Universe = _activeUniverse.Universe };

    EnableCurrentGameObjects();
  }
}
