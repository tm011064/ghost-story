using Assets.Scripts.GhostStory.Behaviours;
using UnityEngine;

public static class GhostStoryMonoBehaviourExtensions
{
  public static Universe GetGameObjectUniverse(this MonoBehaviour monoBehaviour)
  {
    return monoBehaviour.GetComponentOrThrow<LevelObjectConfig>().Universe;
  }
}
