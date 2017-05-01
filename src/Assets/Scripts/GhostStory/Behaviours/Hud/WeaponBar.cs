using System;
using System.Linq;
using UnityEngine;

public class WeaponBar : MonoBehaviour
{
  private SpriteRenderer _spriteRenderer;

  void Start()
  {
    _spriteRenderer = this.GetComponentOrThrow<SpriteRenderer>();

    UpdateIcon(GameManager.Instance.Player);

    GameManager.Instance.PlayerActivated += OnPlayerActivated;
  }

  void OnPlayerActivated(PlayerController player)
  {
    // TODO (Roman): is doing this via events the best/right way?
    UpdateIcon(player);
  }

  void OnDestroy()
  {
    GameManager.Instance.PlayerActivated -= OnPlayerActivated;
  }

  private void UpdateIcon(PlayerController player)
  {
    var activeWeapon = player.GetActiveWeapon();
    if (activeWeapon != null)
    {
      _spriteRenderer.sprite = activeWeapon.HudIcon;
    }

    Logger.UnityDebugLog("UI", activeWeapon);
  }

  void Update()
  {
    if (GameManager.Instance.InputStateManager.IsButtonUp("Right Shoulder"))
    {
      NextWeapon();
    }

    if (GameManager.Instance.InputStateManager.IsButtonUp("Left Shoulder"))
    {
      PreviousWeapon();
    }
  }

  private void NextWeapon()
  {
    SwapWeapon(1);
  }

  private void PreviousWeapon()
  {
    SwapWeapon(-1);
  }

  private void SwapWeapon(int positionsToShift)
  {
    var availableWeapons = GameManager.Instance
      .Player
      .Weapons
      .Select(w => new { Weapon = w, InventoryItem = GhostStoryGameContext.Instance.GameState.GetWeapon(w.Name) })
      .Where(x => x.InventoryItem.IsAvailable)
      .ToArray();

    var index = Array.FindIndex(availableWeapons, w => w.InventoryItem.IsActive);
    if (index >= 0)
    {
      availableWeapons[index].Weapon.gameObject.SetActive(false);
      availableWeapons[index].InventoryItem.IsActive = false;

      var nextIndex = (int)Mathf.Repeat(index + positionsToShift, availableWeapons.Length);

      availableWeapons[nextIndex].Weapon.gameObject.SetActive(true);
      availableWeapons[nextIndex].InventoryItem.IsActive = true;
    }
    else if (availableWeapons.Any())
    {
      var item = positionsToShift > 0
        ? availableWeapons.First()
        : availableWeapons.Last();

      item.Weapon.gameObject.SetActive(true);
      item.InventoryItem.IsActive = true;
    }

    GhostStoryGameContext.Instance.NotifyGameStateChanged();
  }
}
