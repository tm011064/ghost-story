public class WeaponInventoryItemBehaviour : InventoryItemBehaviour
{
  public string WeaponName;

  protected override string GetItemName()
  {
    return WeaponName;
  }
}
