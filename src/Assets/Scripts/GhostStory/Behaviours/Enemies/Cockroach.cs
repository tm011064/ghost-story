using UnityEngine;

public class Cockroach : MovingEnemyController
{
  public override void OnPlayerCollide(PlayerController playerController)
  {

  }

  public override bool CanSpawn()
  {
    var collider = GetComponent<BoxCollider2D>();

    var bounds = GetBounds(collider);

    return !bounds.Intersects(GameManager.Instance.Player.EnemyBoxCollider.bounds);
  }

  public override void Reset()
  {
    ResetControlHandlers(new CockroachControlHandler(this));
  }
}
