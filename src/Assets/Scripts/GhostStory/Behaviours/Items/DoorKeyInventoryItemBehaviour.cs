public class DoorKeyInventoryItemBehaviour : InventoryItemBehaviour
{
  public DoorKey DoorKey;

  protected override string GetItemName()
  {
    return DoorKey.ToString();
  }

  protected override void OnItemAcquired(InventoryItem inventoryItem)
  {
    inventoryItem.IsActive = true;
  }
}
