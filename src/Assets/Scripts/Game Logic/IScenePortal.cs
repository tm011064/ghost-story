using UnityEngine;

public interface IScenePortal
{
  string GetPortalName();

  bool HasName(string name);

  void SpawnPlayer();

  void SpawnPlayerFromPortal(Vector3 fromPortalPosition);

  bool CanSpawn();
}
