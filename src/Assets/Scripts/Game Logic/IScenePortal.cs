using UnityEngine;

public interface IScenePortal
{
  void SpawnPlayer(PlayerController playerController, Vector3? cameraPosition = null);
}
