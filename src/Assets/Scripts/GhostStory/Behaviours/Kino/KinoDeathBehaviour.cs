public class KinoDeathBehaviour : PlayerDeathBehaviour
{
  protected override void OnPlayerDied()
  {
    Logger.Info("Kino died, loading from portal " + GhostStoryGameContext.Instance.CheckpointManager.CheckpointName);

    GameManager.Instance.SceneManager.OnSceneLoad();
  }
}
