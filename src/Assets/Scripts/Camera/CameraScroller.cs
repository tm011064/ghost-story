﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CameraScroller : MonoBehaviour
{
  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  public FullScreenScrollSettings FullScreenScrollSettings;

  [Tooltip("The (x, y) offset of the camera. This can be used when default vertical locking is disabled and you want the player to be below, above, right or left of the screen center.")]
  public Vector2 Offset;

  [Tooltip("The dimensions of the camera boundaries")]
  public Vector2 Size;

  public float HorizontalOffsetDeltaMovementFactor = 40f;

  public VerticalCameraFollowMode VerticalCameraFollowMode;

  public FullScreenScrollerTransitionMode FullScreenScrollerTransitionMode
    = FullScreenScrollerTransitionMode.FirstVerticalThenHorizontal;

  [Tooltip("Defines the vertical speed when using FullScreenScrollerTransitionMode.FirstVerticalThenHorizontal as a factor of the horizontal translation speed")]
  public float VerticalFullScreenScrollerTransitionSpeedFactor = 4f;

  protected CameraMovementSettings CameraMovementSettings;

  protected CameraController CameraController;

  private GameObject _parent;

  private int _animationShortNameHash;

  private Checkpoint _checkpoint;

  protected TranslateTransformActions ScrollActions;

  protected ScrollStatus Status = ScrollStatus.Idle;

  void Awake()
  {
    CameraController = Camera.main.GetComponent<CameraController>();
    CameraController.ScrollActionCompleted += OnScrollActionCompleted;

    CameraMovementSettings = CreateCameraMovementSettings();

    var enterTriggers = GetComponentsInChildren<ITriggerEnterExit>();
    foreach (var enterTrigger in enterTriggers)
    {
      enterTrigger.Entered += (_, e) => OnEnter(e.SourceCollider);
      enterTrigger.Exited += (_, e) => CameraController.OnCameraModifierExit(CameraMovementSettings);
    }

    _checkpoint = GetComponentInChildren<Checkpoint>();

    OnAwake();
  }

  protected virtual void OnAwake()
  {
  }

  void OnDestroy()
  {
    CameraController.ScrollActionCompleted -= OnScrollActionCompleted;

    var enterTriggers = GetComponentsInChildren<ITriggerEnterExit>();
    foreach (var enterTrigger in enterTriggers)
    {
      enterTrigger.Entered -= (_, e) => OnEnter(e.SourceCollider);
      enterTrigger.Exited -= (_, e) => CameraController.OnCameraModifierExit(CameraMovementSettings);
    }
  }

  private CameraMovementSettings CreateCameraMovementSettings()
  {
    var horizontalLockSettings = CreateHorizontalLockSettings();

    var verticalLockSettings = CreateVerticalLockSettings();

    return new CameraMovementSettings(
      verticalLockSettings,
      horizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      VerticalCameraFollowMode,
      HorizontalOffsetDeltaMovementFactor);
  }

  private void StartScroll(Collider2D collider)
  {
    // the order here is important. First we want to set the camera movement settings, then we can create
    // the scroll transform action.
    var targetPosition = CameraController.CalculateTargetPosition(CameraMovementSettings);
    var player = GameManager.Instance.Player;
    var cameraPosition = CameraController.Transform.position;

    var contexts = PlayerTranslationActionContextFactory.Create(
      cameraPosition,
      targetPosition,
      FullScreenScrollerTransitionMode,
      _animationShortNameHash,
      FullScreenScrollSettings,
      VerticalFullScreenScrollerTransitionSpeedFactor).ToArray();

    ScrollActions = new TranslateTransformActions(contexts.Select(c => c.TranslateTransformAction));

    CameraController.RegisterScrollActions(ScrollActions);

    var playerControlHandlersStack = new Stack<PlayerControlHandler>(
      contexts.Select(c => c.PlayerControlHandler));

    if (FullScreenScrollSettings.EndScrollFreezeTime > 0f)
    {
      playerControlHandlersStack.Push(new FreezePlayerControlHandler(
        player,
        FullScreenScrollSettings.EndScrollFreezeTime,
        _animationShortNameHash,
        new PlayerState[] { PlayerState.Locked, PlayerState.Invincible }));
    }

    player.ExchangeActiveControlHandler(playerControlHandlersStack.Pop());
    player.PushControlHandlers(playerControlHandlersStack.ToArray());
  }

  void OnScrollActionCompleted(TranslateTransformActions translateTransformActions)
  {
    if (Status != ScrollStatus.Scrolling
      || ScrollActions != translateTransformActions)
    {
      return;
    }

    ScrollActions = null;

    OnCameraScrollCompleted();

    Status = ScrollStatus.Idle;
  }

  protected virtual void OnCameraScrollCompleted()
  {
  }

  protected void OnEnter(Collider2D collider)
  {
    if (Status == ScrollStatus.Scrolling)
    {
      return;
    }

    UpdatePlayerSpawnLocation();

    if (CameraController.IsCurrentCameraMovementSettings(CameraMovementSettings))
    {
      return;
    }

    Status = ScrollStatus.Scrolling;

    var currentAnimatorStateInfo = GameManager.Instance.Player.Animator.GetCurrentAnimatorStateInfo(0);

    _animationShortNameHash = currentAnimatorStateInfo.shortNameHash;

    if (FullScreenScrollSettings.StartScrollFreezeTime > 0f)
    {
      var delay = TimeSpan.FromSeconds(FullScreenScrollSettings.StartScrollFreezeTime);

      GameManager.Instance.Player.PushControlHandler(
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          FullScreenScrollSettings.StartScrollFreezeTime + (float)delay.TotalSeconds,
          _animationShortNameHash,
          new PlayerState[] { PlayerState.Locked, PlayerState.Invincible }));

      this.Invoke(delay, () => StartScroll(collider));
    }
    else
    {
      StartScroll(collider);
    }
  }

  private void UpdatePlayerSpawnLocation()
  {
    if (_checkpoint != null)
    {
      GameManager.Instance.Player.SpawnLocation = _checkpoint.transform.position;
    }
  }

  private VerticalLockSettings CreateVerticalLockSettings()
  {
    var verticalLockSettings = new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = false,
      DefaultVerticalLockPosition = 0f,
      EnableTopVerticalLock = true,
      EnableBottomVerticalLock = true,
      TopVerticalLockPosition = transform.position.y + Size.y * .5f,
      BottomVerticalLockPosition = transform.position.y - Size.y * .5f
    };

    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - CameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + CameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    return verticalLockSettings;
  }

  private HorizontalLockSettings CreateHorizontalLockSettings()
  {
    var horizontalLockSettings = new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = transform.position.x - Size.x * .5f,
      RightHorizontalLockPosition = transform.position.x + Size.x * .5f
    };

    horizontalLockSettings.LeftBoundary =
      horizontalLockSettings.LeftHorizontalLockPosition
      + CameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      horizontalLockSettings.RightHorizontalLockPosition
      - CameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    return horizontalLockSettings;
  }

  protected enum ScrollStatus
  {
    Scrolling,
    Idle
  }
}