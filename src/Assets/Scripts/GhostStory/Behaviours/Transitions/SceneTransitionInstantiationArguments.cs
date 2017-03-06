using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public class SceneTransitionInstantiationArguments : AbstractInstantiationArguments
  {
    public static SceneTransitionInstantiationArguments FromPrefabInstantiationArguments(
      PrefabInstantiationArguments prefabInstantiationArguments)
    {
      var doorKey = (DoorKey)Enum.Parse(typeof(DoorKey), prefabInstantiationArguments.Properties["Door Key"]);
      var doorLocation = (HorizontalDirection)Enum.Parse(typeof(HorizontalDirection), prefabInstantiationArguments.Properties["Transition Direction"]);

      var cameraBounds = prefabInstantiationArguments.WrappingCameraBounds.FirstOrDefault();

      return new SceneTransitionInstantiationArguments
      {
        CameraBounds = cameraBounds,
        DoorKey = doorKey,
        DoorLocation = doorLocation,
        Position = prefabInstantiationArguments.TiledRectBounds.center,
        PrefabsAssetPathsByShortName = prefabInstantiationArguments.PrefabsAssetPathsByShortName,
        TransitionToPortalName = prefabInstantiationArguments.Properties["Transition To Portal"],
        TransitionToScene = prefabInstantiationArguments.Properties["Transition To Scene"],
        PortalName = prefabInstantiationArguments.TiledObjectName,
      };
    }

    public Vector3 Position;

    public Bounds CameraBounds;

    public DoorKey DoorKey;

    public HorizontalDirection DoorLocation;

    public string TransitionToScene;

    public string TransitionToPortalName;

    public string PortalName;
  }
}
