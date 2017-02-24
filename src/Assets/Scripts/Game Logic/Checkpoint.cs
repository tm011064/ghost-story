using UnityEngine;

public partial class Checkpoint : MonoBehaviour, IScenePortal
{
  public string PortalName;

  void OnTriggerEnter2D(Collider2D collider)
  {
    GameManager.Instance.Player.SpawnLocation = transform.position;
  }

  public string GetPortalName()
  {
    return PortalName;
  }

  public void SpawnPlayer()
  {
    GameManager.Instance.Player.transform.position = transform.position;
  }

  public void SpawnPlayerFromPortal(Vector3 fromPortalPosition)
  {
    SpawnPlayer();
  }
}
