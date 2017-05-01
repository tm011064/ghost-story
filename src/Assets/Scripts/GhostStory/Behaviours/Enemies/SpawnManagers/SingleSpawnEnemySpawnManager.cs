public class SingleSpawnEnemySpawnManager : AbstractEnemySpawnManager
{
  protected override void OnSpawnCompleted()
  {
    if (SpawnWhenBecameVisible)
    {
      StopVisibilityChecks();
    }
  }
}
