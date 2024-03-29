﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PauseMenuCanvas : MonoBehaviour
{
  public GameObject PauseMenuItem;

  private PauseMenuItemGroup[] _itemGroups;

  private PauseMenuItemGroup _focusedMenuItemGroup;

  private PlayerState _playerStateWhenEnabled;

  void Start()
  {
    _itemGroups = BuildItems();
  }

  void OnEnable()
  {
    LockPlayer();

    GhostStoryGameContext.Instance.DisableCurrentGameObjects();

    Time.timeScale = 0f;
  }

  void OnDisable()
  {
    UnlockPlayer();

    GhostStoryGameContext.Instance.EnableCurrentGameObjects();

    Time.timeScale = 1f;
  }

  private void LockPlayer()
  {
    _playerStateWhenEnabled = GameManager.Instance.Player.PlayerState;
    GameManager.Instance.Player.PlayerState |= PlayerState.Locked;
  }

  private void UnlockPlayer()
  {
    GameManager.Instance.Player.PlayerState = _playerStateWhenEnabled;
  }

  private PauseMenuItemGroup[] BuildItems()
  {
    var itemGroups = GetItemGroups().ToArray();

    ArrangeMenuItemGroups(itemGroups);

    _focusedMenuItemGroup = itemGroups.First();
    _focusedMenuItemGroup.SelectedIndex = 0;

    return itemGroups;
  }

  private IEnumerable<PauseMenuItemGroup> GetItemGroups()
  {
    var gameState = GhostStoryGameContext.Instance.GameState;

    var weapons = GameManager.Instance
      .GetPlayerControllers()
      .Select(p =>
        CreateMenuItemGroup(
          CreateMenuItems(gameState.Weapons.Where(w => p.Weapons.Any(pw => string.Equals(pw.Name, w.Name))).ToArray()).ToArray(),
          false));

    foreach (var weapon in weapons)
    {
      yield return weapon;
    }

    yield return CreateMenuItemGroup(CreateMenuItems(GhostStoryGameContext.Instance.GameState.DoorKeys).ToArray(), true);
  }

  private void ArrangeMenuItemGroups(PauseMenuItemGroup[] itemGroups)
  {
    var columnIndex = -1;
    var rowIndex = 0;

    var maxWidth = itemGroups
      .SelectMany(ig => ig.PauseMenuItems)
      .Max(mi => mi.RectTransform.sizeDelta.x);

    var rowPosY = 0f;
    var maxHeight = float.MinValue;

    foreach (var itemGroup in itemGroups)
    {
      var pos = new Vector3(
        maxWidth * columnIndex,
        rowPosY,
        itemGroup.RectTransform.position.z);
            
      itemGroup.RectTransform.anchoredPosition = pos;

      Logger.UnityDebugLog("pos", itemGroup.RectTransform.position, "ind", maxWidth * columnIndex, "w", maxWidth);
      itemGroup.RectTransform.sizeDelta = itemGroup.RectTransform.sizeDelta.SetX(maxWidth);

      maxHeight = Mathf.Max(
        maxHeight,
        itemGroup.PauseMenuItems.Sum(r => r.RectTransform.sizeDelta.y));

      columnIndex++;
      if (columnIndex > 1)
      {
        columnIndex = 0;
        rowIndex++;

        rowPosY += maxHeight;
        maxHeight = float.MinValue;
      }
    }
  }

  private PauseMenuItemGroup CreateMenuItemGroup(PauseMenuItem[] items, bool isMultiSelect)
  {
    var gameObject = new GameObject("Menu Item Group");
    gameObject.transform.parent = transform;

    var groupRectTransform = gameObject.AddComponent<RectTransform>();
    groupRectTransform.localScale = new Vector3(1, 1, 1);

    var itemPosition = Vector3.zero;
    foreach (var item in items)
    {
      item.RectTransform.position = itemPosition;

      item.transform.SetParent(gameObject.transform, false);

      itemPosition = itemPosition.SetY(itemPosition.y - item.RectTransform.sizeDelta.y);
    }

    return new PauseMenuItemGroup
    {
      PauseMenuItems = items,
      RectTransform = groupRectTransform,
      IsMultiSelect = isMultiSelect
    };
  }

  private IEnumerable<PauseMenuItem> CreateMenuItems(IEnumerable<InventoryItem> inventoryItems)
  {
    foreach (var inventoryItem in inventoryItems)
    {
      var pauseMenuItemGameObject = GameObject.Instantiate(PauseMenuItem, Vector3.zero, Quaternion.identity) as GameObject;

      var pauseMenuItem = pauseMenuItemGameObject.GetComponent<PauseMenuItem>();

      pauseMenuItem.InventoryItem = inventoryItem;
      pauseMenuItem.Text.text = inventoryItem.Name;

      yield return pauseMenuItem;
    }
  }

  void Update()
  {
    if (GameManager.Instance.InputStateManager.IsButtonDown("Menu Exit"))
    {
      GhostStoryGameContext.Instance.NotifyGameStateChanged();
      gameObject.SetActive(false);
      return;
    }

#if DEBUG // TODO (Important): remove eventually
    if (GameManager.Instance.InputStateManager.IsButtonDown("Menu Debug Toggle Available"))
    {
      _focusedMenuItemGroup.OnSelectedItemToggleAvailable();
      return;
    }
#endif

    if (GameManager.Instance.InputStateManager.IsButtonDown("Menu Select"))
    {
      _focusedMenuItemGroup.OnSelectedItemClick();
      return;
    }

    if (GameManager.Instance.InputStateManager.IsDownAxisButtonDown(GameManager.Instance.InputSettings))
    {
      _focusedMenuItemGroup.SelectedIndex++;
    }
    else if (GameManager.Instance.InputStateManager.IsUpAxisButtonDown(GameManager.Instance.InputSettings))
    {
      _focusedMenuItemGroup.SelectedIndex--;
    }
    else if (GameManager.Instance.InputStateManager.IsRightAxisButtonDown(GameManager.Instance.InputSettings))
    {
      if (_focusedMenuItemGroup != _itemGroups.Last())
      {
        _focusedMenuItemGroup.UnselectAll();

        _focusedMenuItemGroup = _itemGroups[Array.IndexOf(_itemGroups, _focusedMenuItemGroup) + 1];
        _focusedMenuItemGroup.SelectedIndex = 0;
      }
    }
    else if (GameManager.Instance.InputStateManager.IsLeftAxisButtonDown(GameManager.Instance.InputSettings))
    {
      if (_focusedMenuItemGroup != _itemGroups.First())
      {
        _focusedMenuItemGroup.UnselectAll();

        _focusedMenuItemGroup = _itemGroups[Array.IndexOf(_itemGroups, _focusedMenuItemGroup) - 1];
        _focusedMenuItemGroup.SelectedIndex = 0;
      }
    }
  }

  private class PauseMenuItemGroup
  {
    public bool IsFocused { get; set; }

    public PauseMenuItem SelectedItem { get { return PauseMenuItems[SelectedIndex]; } }

    public PauseMenuItem[] PauseMenuItems { get; set; }

    public RectTransform RectTransform { get; set; }

    public bool IsMultiSelect { get; set; }

#if DEBUG // TODO (Important): remove eventually
    public void OnSelectedItemToggleAvailable()
    {
      SelectedItem.InventoryItem.IsAvailable = !SelectedItem.InventoryItem.IsAvailable;
      SelectedItem.InventoryItem.IsActive = false;
    }
#endif

    public void OnSelectedItemClick()
    {
      if (!IsMultiSelect)
      {
        foreach (var item in PauseMenuItems.Where(i => i != SelectedItem))
        {
          item.InventoryItem.IsActive = false;
        }
      }

      SelectedItem.InventoryItem.IsActive = !SelectedItem.InventoryItem.IsActive;
    }

    private int _selectedIndex = 0;
    public int SelectedIndex
    {
      get { return _selectedIndex; }
      set
      {
        PauseMenuItems[_selectedIndex].Selected = false;

        _selectedIndex = Mathf.Max(
          Mathf.Min(
            PauseMenuItems.Length - 1,
            value),
          0);

        PauseMenuItems[_selectedIndex].Selected = true;
      }
    }

    public void UnselectAll()
    {
      foreach (var item in PauseMenuItems)
      {
        item.Selected = false;
      }
    }
  }
}
