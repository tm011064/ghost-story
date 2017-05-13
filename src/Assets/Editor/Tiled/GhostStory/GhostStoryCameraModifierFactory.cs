using System.Collections.Generic;
using Assets.Editor.Tiled.GameObjectFactories;
using Assets.Editor.Tiled.Xml;
using UnityEngine;

namespace Assets.Editor.Tiled.GhostStory
{
  public class GhostStoryCameraModifierFactory : CameraModifierFactory
  {
    public GhostStoryCameraModifierFactory(
      GameObject root,
      Map map,
      Dictionary<string, string> prefabLookup,
      Dictionary<string, ObjectType> objectTypesByName)
      : base(root, map, prefabLookup, objectTypesByName)
    {
    }

    protected override void OnGameObjectCreated(AbstractTiledLayerConfig layerConfig, GameObject gameObject)
    {
      gameObject.AddLevelConfigComponent(layerConfig);
    }
  }
}
