public class GhostStoryCheckpointManager
{
  public string CheckpointName;

  public void ClearCheckpoint()
  {
    CheckpointName = null;
  }

  public bool HasCheckpoint()
  {
    return !string.IsNullOrEmpty(CheckpointName);
  }
}
