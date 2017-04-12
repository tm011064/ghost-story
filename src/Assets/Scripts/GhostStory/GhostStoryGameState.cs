using System;
using System.Linq;

[Serializable]
public class GhostStoryGameState
{
  public Universe ActiveUniverse;

  public InventoryItem[] Weapons;

  public InventoryItem[] DoorKeys;

  public int MisaHealthUnits;

  public int KinoHealthUnits;

  public string SpawnPlayerName;

  public string SpawnPlayerPortalName;

  public string SpawnPlayerSceneName;

  public InventoryItem GetWeapon(string name)
  {
    return Weapons.Single(w => w.Name == name);
  }

  public InventoryItem GetDoorKey(DoorKey doorKey)
  {
    return DoorKeys.Single(w => w.Name == doorKey.ToString());
  }

  public InventoryItem Find(string name)
  {
    var allItems = Weapons.Union(DoorKeys).ToArray();

    return allItems.First(i => i.Name == name);
  }

  public override string ToString()
  {
    return this.GetFieldValuesFormatted();
  }
}
