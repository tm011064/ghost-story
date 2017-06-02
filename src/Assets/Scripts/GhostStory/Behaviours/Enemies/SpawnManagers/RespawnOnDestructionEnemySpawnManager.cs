using UnityEngine;

public class RespawnOnDestructionEnemySpawnManager : AbstractEnemySpawnManager
{
  public float RespawnOnDestroyDelay = 1;

  protected override void OnSpawnFailed()
  {
    Respawn();
  }

  protected override void OnEnemyDisabled()
  {
    Respawn();
  }

  private void Respawn()
  {
    if (!isActiveAndEnabled)
    {
      return;
    }

    try
    {
      GhostStoryGameContext.Instance.RegisterCallback(
        RespawnOnDestroyDelay, 
        Spawn,
        this.GetGameObjectUniverse(), 
        SpawnTimerName);
    }
    catch (MissingReferenceException)
    {
      // this can happen on scene unload when a spawned enemy disables after this object has been finalized
    }
  }
}
