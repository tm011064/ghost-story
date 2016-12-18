#if UNITY_EDITOR

using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class FrontDoor : IInstantiable<LayerTransitionInstantiationArguments>
  {
    public void Instantiate(LayerTransitionInstantiationArguments arguments)
    {
      transform.position = arguments.TransitionObjectBounds.center;
      TransitionsToLayer = arguments.TransitionToLayer.ToEnum<LevelLayer>();
    }
  }
}

#endif