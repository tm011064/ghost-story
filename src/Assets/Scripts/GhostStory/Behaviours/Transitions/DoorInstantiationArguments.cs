using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public class DoorInstantiationArguments : AbstractInstantiationArguments
  {
    public Bounds CameraBounds;

    public Bounds TriggerBounds;

    public DoorKey DoorKey;

    public Direction DoorLocation;
  }
}
