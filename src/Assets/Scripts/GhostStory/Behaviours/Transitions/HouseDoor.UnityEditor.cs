#if UNITY_EDITOR

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class HouseDoor : IInstantiable<InstantiationArguments>
  {
    public void Instantiate(InstantiationArguments arguments)
    {
      var center = arguments.Bounds.center;
      transform.position = center;

      TransitionToScene = arguments.Properties["Transition To Scene"];
      TransitionToSceneObject = arguments.Properties["Transition To Portal"];
    }
  }
}

#endif