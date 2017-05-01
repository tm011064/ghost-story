using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CameraScroller : CameraMovementSettingsBehaviour
{
  public FullScreenScrollSettings FullScreenScrollSettings;

  [Tooltip("The dimensions of the camera boundaries")]
  public Vector2 Size;

  private GameObject _parent;

  private int _animationShortNameHash;

  private Checkpoint _checkpoint;

  protected TranslateTransformActions ScrollActions;

  protected ScrollStatus Status = ScrollStatus.Idle;

  protected void Awake()
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

  protected virtual void OnDestroy()
  {
    CameraController.ScrollActionCompleted -= OnScrollActionCompleted;

    var enterTriggers = GetComponentsInChildren<ITriggerEnterExit>();
    foreach (var enterTrigger in enterTriggers)
    {
      enterTrigger.Entered -= (_, e) => OnEnter(e.SourceCollider);
      enterTrigger.Exited -= (_, e) => CameraController.OnCameraModifierExit(CameraMovementSettings);
    }
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
      _animationShortNameHash,
      FullScreenScrollSettings).ToArray();

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

  protected enum ScrollStatus
  {
    Scrolling,
    Idle
  }

  protected override VerticalLockSettings CreateBaseVerticalLockSettings()
  {
    return new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = false,
      DefaultVerticalLockPosition = 0f,
      EnableTopVerticalLock = true,
      EnableBottomVerticalLock = true,
      TopVerticalLockPosition = transform.position.y + Size.y * .5f,
      BottomVerticalLockPosition = transform.position.y - Size.y * .5f
    };
  }

  protected override HorizontalLockSettings CreateBaseHorizontalLockSettings()
  {
    return new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = transform.position.x - Size.x * .5f,
      RightHorizontalLockPosition = transform.position.x + Size.x * .5f
    };
  }
}