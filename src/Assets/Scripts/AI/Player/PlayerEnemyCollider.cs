using UnityEngine;

public class PlayerEnemyCollider : MonoBehaviour
{
  private void HandlePlayerCollision(Collider2D collider)
  {
    var enemyController = collider.gameObject.GetComponent<IPlayerCollidable>();

    if (enemyController != null)
    {
      enemyController.OnPlayerCollide(GameManager.Instance.Player);
    }
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    HandlePlayerCollision(collider);
  }

  void OnTriggerStay2D(Collider2D collider)
  {
    if ((GameManager.Instance.Player.PlayerState & PlayerState.Invincible) != 0)
    {
      return;
    }

    HandlePlayerCollision(collider);
  }
}
