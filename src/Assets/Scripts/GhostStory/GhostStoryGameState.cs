using System;
using System.Linq;

[Serializable]
public class GhostStoryGameState
{
  public LayerUniverseKey ActiveUniverse;

  public InventoryItem[] Weapons;

  public InventoryItem[] DoorKeys;

  public int MisaHealthUnits;

  public int KinoHealthUnits;

  public InventoryItem GetWeapon(string name)
  {
    return Weapons.Single(w => w.Name == name);
  }

  public InventoryItem GetDoorKey(string name)
  {
    return DoorKeys.Single(w => w.Name == name);
  }

  public InventoryItem Find(string name)
  {
    var allItems = Weapons.Union(DoorKeys).ToArray();

    Logger.UnityDebugLog(allItems.Where(i => i.Name == name).FirstOrDefault().Name);

    return allItems.First(i => i.Name == name);
  }
}
