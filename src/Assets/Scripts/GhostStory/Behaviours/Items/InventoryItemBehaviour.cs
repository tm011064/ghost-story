using UnityEngine;

public partial class InventoryItemBehaviour : MonoBehaviour
{
  public string ItemName;

  void Start()
  {
    GhostStoryGameContext.Instance.GameStateChanged += OnGameStateChanged;

    EvaluateActive();
  }

  void OnDestroy()
  {
    GhostStoryGameContext.Instance.GameStateChanged -= OnGameStateChanged;
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (!isActiveAndEnabled)
    {
      return;
    }

    var inventoryItem = GhostStoryGameContext.Instance.GameState.Find(ItemName);

    GhostStoryGameContext.Instance.OnInventoryItemAcquired(inventoryItem);
  }

  void OnGameStateChanged(GhostStoryGameState obj)
  {
    EvaluateActive();
  }

  private void EvaluateActive()
  {
    var inventoryItem = GhostStoryGameContext.Instance.GameState.Find(ItemName);

    gameObject.SetActive(!inventoryItem.IsAvailable);
  }
}
