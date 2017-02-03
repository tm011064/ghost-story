using UnityEngine;

public interface IScenePortal
{
  string GetPortalName();

  void SpawnPlayer();

  void SpawnPlayerFromPortal(Vector3 fromPortalPosition);
}
