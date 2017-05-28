#if UNITY_EDITOR

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class FrontPortal : IInstantiable<PrefabInstantiationArguments>
  {
    public void Instantiate(PrefabInstantiationArguments arguments)
    {
      arguments.CheckHasTiledObjectName();
      arguments.CheckHasProperties("Transition To Portal", "Transition To Scene");

      transform.position = arguments.TiledRectBounds.center;

      TransitionToPortalName = arguments.Properties["Transition To Portal"];
      TransitionToScene = arguments.Properties["Transition To Scene"];

      PortalName = arguments.TiledObjectName;
    }
  }
}

#endif