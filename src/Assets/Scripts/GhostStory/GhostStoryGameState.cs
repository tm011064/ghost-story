using System;
using System.Linq;

[Serializable]
public class GhostStoryGameState
{
  public LayerUniverseKey ActiveUniverse;

  public InventoryItem[] Weapons;

  public InventoryItem[] DoorKeys;

  public InventoryItem GetWeapon(string name)
  {
    return Weapons.Single(w => w.Name == name);
  }
}
