using UnityEngine;

public abstract partial class InventoryItemBehaviour : MonoBehaviour
{
  protected abstract string GetItemName();

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

    var inventoryItem = GhostStoryGameContext.Instance.GameState.Find(GetItemName());

    GhostStoryGameContext.Instance.OnInventoryItemAcquired(inventoryItem);

    OnItemAcquired(inventoryItem);
  }

  protected virtual void OnItemAcquired(InventoryItem inventoryItem)
  {
  }

  void OnGameStateChanged(GhostStoryGameState obj)
  {
    EvaluateActive();
  }

  private void EvaluateActive()
  {
    var inventoryItem = GhostStoryGameContext.Instance.GameState.Find(GetItemName());

    gameObject.SetActive(!inventoryItem.IsAvailable);
  }
}
