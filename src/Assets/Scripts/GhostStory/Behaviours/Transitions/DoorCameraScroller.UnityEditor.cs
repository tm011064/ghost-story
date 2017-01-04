#if UNITY_EDITOR

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class DoorCameraScroller : IInstantiable<DoorInstantiationArguments>
  {
    public void Instantiate(DoorInstantiationArguments arguments)
    {
      SetPosition(arguments.CameraBounds);

      var boxColliderGameObject = CreateBoxColliderGameObject(arguments.TriggerBounds, "Door Handle Trigger");

      var triggerEnterBehaviour = boxColliderGameObject.AddComponent<DoorTriggerEnterBehaviour>();
      triggerEnterBehaviour.DoorKeysNeededToEnter = new DoorKey[] { arguments.DoorKey };
      triggerEnterBehaviour.DoorLocation = arguments.DoorLocation;
    }
  }
}

#endif