using Assets.Scripts.GhostStory.Behaviours;
using UnityEngine;

public static class GhostStoryMonoBehaviourExtensions
{
  public static Universe GetGameObjectUniverse(this MonoBehaviour monoBehaviour)
  {
    return monoBehaviour.GetComponentOrThrow<LevelObjectConfig>().Universe;
  }

  public static Universe GetGameObjectUniverseRecursive(this MonoBehaviour monoBehaviour)
  {
    var levelObjectConfig = monoBehaviour.GetComponent<LevelObjectConfig>();
    if (levelObjectConfig != null)
    {
      return levelObjectConfig.Universe;
    }

    var parent = monoBehaviour.transform.parent;
    while (parent != null)
    {
      levelObjectConfig = parent.GetComponent<LevelObjectConfig>();
      if (levelObjectConfig != null)
      {
        return levelObjectConfig.Universe;
      }

      parent = parent.parent;
    }

    throw new MissingComponentException(
      "Monobehaviour " + monoBehaviour.name + " at object " + monoBehaviour.gameObject.name 
      + " missing LevelObjectConfig in hierarchy");
  }
}
