using UnityEngine;

public class GhostStoryGameManager : GameManager
{
  protected override void OnAwake()
  {
    GhostStoryGameContext.Instance = new GhostStoryGameContext(
      GameObject.FindGameObjectsWithTag("RealWorld"),
      GameObject.FindGameObjectsWithTag("AlternateWorld"));
  }
}
