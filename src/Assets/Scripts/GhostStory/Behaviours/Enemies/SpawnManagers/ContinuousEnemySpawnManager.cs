using System;

public class ContinuousEnemySpawnManager : AbstractEnemySpawnManager
{
  public float ContinuousSpawnInterval = 1;

  protected override void OnSpawnCompleted()
  {
    if (SpawnWhenBecameVisible)
    {
      StopVisibilityChecks(); // TODO (Roman): check whether we need to avoid multiple calls here
    }

    if (ContinuousSpawnInterval <= 0)
    {
      throw new ArgumentException("ContinuousSpawnInterval");
    }

    GhostStoryGameContext.Instance.RegisterCallback(ContinuousSpawnInterval, Spawn, "Spawn");
  }
}
