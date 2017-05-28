using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public class SceneTransitionInstantiationArguments : AbstractInstantiationArguments
  {
    public static SceneTransitionInstantiationArguments FromPrefabInstantiationArguments(
      PrefabInstantiationArguments arguments)
    {
      arguments.CheckHasTiledObjectName();
      arguments.CheckHasProperties(
        "Transition To Portal",
        "Transition To Scene",
        "Door Key",
        "Transition Direction");

      var doorKey = (DoorKey)Enum.Parse(typeof(DoorKey), arguments.Properties["Door Key"]);
      var doorLocation = (HorizontalDirection)Enum.Parse(typeof(HorizontalDirection), arguments.Properties["Transition Direction"]);

      var cameraBounds = arguments.WrappingCameraBounds.FirstOrDefault();

      return new SceneTransitionInstantiationArguments
      {
        CameraBounds = cameraBounds,
        DoorKey = doorKey,
        DoorLocation = doorLocation,
        Position = arguments.TiledRectBounds.center,
        PrefabsAssetPathsByShortName = arguments.PrefabsAssetPathsByShortName,
        TransitionToPortalName = arguments.Properties["Transition To Portal"],
        TransitionToScene = arguments.Properties["Transition To Scene"],
        PortalName = arguments.TiledObjectName,
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
