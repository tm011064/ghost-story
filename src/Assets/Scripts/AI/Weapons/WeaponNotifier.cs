using System.Linq;
using UnityEngine;

public class WeaponNotifier : MonoBehaviour
{
  public void StopAttack()
  {
    var attackingWeapons = GameManager.Instance
      .Player
      .Weapons
      .Where(w => w.IsAttacking());

    foreach (var weapon in attackingWeapons)
    {
      weapon.StopAttack();
    }
  }
}
