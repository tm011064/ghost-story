using System;
using Assets.Scripts.GhostStory.Behaviours;
using UnityEngine;

public partial class Checkpoint : MonoBehaviour, IScenePortal, ISceneCheckpoint
{
  public string CheckpointName;

  public bool CanSpawn()
  {
    var config = GetComponent<LevelObjectConfig>();

    return GhostStoryGameContext.Instance.GameState.ActiveUniverse == config.Universe;
  }

  public string GetPortalName()
  {
    return CheckpointName;
  }

  public bool HasName(string name)
  {
    return string.Equals(CheckpointName, name, StringComparison.OrdinalIgnoreCase);
  }

  public void SpawnPlayer()
  {
    GameManager.Instance.Player.transform.position = transform.position;
    GameManager.Instance.Player.CharacterPhysicsManager.WarpToFloor();

    GameManager.Instance.SceneManager.FadeIn();

    GameManager.Instance.SceneManager.FocusCameraOnPlayer();
  }

  public void SpawnPlayerFromPortal(Vector3 fromPortalPosition)
  {
    SpawnPlayer();
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    GhostStoryGameContext.Instance.CheckpointManager.CheckpointName = CheckpointName;
  }
}
