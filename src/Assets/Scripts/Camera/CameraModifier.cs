﻿using System;
using UnityEngine;

public partial class CameraModifier : MonoBehaviour, ICameraModifier
{
  public HorizontalCamereaWindowSettings HorizontalCamereaWindowSettings;

  public VerticalSnapWindowSettings VerticalSnapWindowSettings;

  public VerticalLockSettings VerticalLockSettings;

  public HorizontalLockSettings HorizontalLockSettings;

  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  public CameraSettings CameraSettings;

  public Color GizmoColor = Color.magenta;

  public bool MustBeOnLadderToEnter;

  private CameraController _cameraController;

  private CameraMovementSettings _cameraMovementSettings;

  void Awake()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();
    _cameraMovementSettings = CreateCameraMovementSettings();

    var triggerEnterBehaviours = GetComponentsInChildren<ITriggerEnterExit>();
    foreach (var triggerEnterBehaviour in triggerEnterBehaviours)
    {
      triggerEnterBehaviour.Entered += OnEnterTriggerInvoked;
      triggerEnterBehaviour.Exited += OnExitTriggerInvoked;
    }

    OnAwake();
  }

  public void Activate()
  {
    _cameraController.OnCameraModifierEnter(_cameraMovementSettings);
  }

  protected void OverrideSettings(
    SmoothDampMoveSettings smoothDampMoveSettings,
    CameraSettings cameraSettings,
    HorizontalCamereaWindowSettings horizontalCamereaWindowSettings,
    VerticalSnapWindowSettings verticalSnapWindowSettings)
  {
    CameraSettings = cameraSettings;
    SmoothDampMoveSettings = smoothDampMoveSettings;
    VerticalSnapWindowSettings = verticalSnapWindowSettings;
    HorizontalCamereaWindowSettings = horizontalCamereaWindowSettings;

    _cameraMovementSettings = CreateCameraMovementSettings();
  }

  protected virtual void OnAwake()
  {
  }

  private void SetHorizontalBoundaries(HorizontalLockSettings horizontalLockSettings, CameraController cameraController)
  {
    horizontalLockSettings.LeftBoundary =
      horizontalLockSettings.LeftHorizontalLockPosition
      + cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      horizontalLockSettings.RightHorizontalLockPosition
      - cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;
  }

  private void SetVerticalBoundaries(VerticalLockSettings verticalLockSettings, CameraController cameraController)
  {
    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;
  }

  protected CameraMovementSettings CreateCameraMovementSettings()
  {
    if (ZoomSettings.ZoomPercentage == 0f)
    {
      throw new ArgumentOutOfRangeException("Zoom Percentage must not be 0.");
    }

    SetVerticalBoundaries(VerticalLockSettings, _cameraController);
    SetHorizontalBoundaries(HorizontalLockSettings, _cameraController);

    return new CameraMovementSettings(
      HorizontalCamereaWindowSettings,
      VerticalSnapWindowSettings,
      VerticalLockSettings,
      HorizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      CameraSettings);
  }

  public bool Contains(Vector2 point)
  {
    var cameraMovementSettings = new CameraMovementSettings(
      HorizontalCamereaWindowSettings,
      VerticalSnapWindowSettings,
      VerticalLockSettings,
      HorizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      CameraSettings);

    return cameraMovementSettings.Contains(point);
  }

  public void TryForceTrigger()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();
    _cameraMovementSettings = CreateCameraMovementSettings();

    var playerPosition = GameManager.Instance.Player.transform.position;
    if (_cameraMovementSettings.Contains(playerPosition))
    {
      _cameraController.OnCameraModifierEnter(_cameraMovementSettings);
      _cameraController.MoveCameraToTargetPosition();
    }
  }

  void OnEnterTriggerInvoked(object sender, TriggerEnterExitEventArgs e)
  {
    if (MustBeOnLadderToEnter
      && (GameManager.Instance.Player.PlayerState & PlayerState.ClimbingLadder) == 0)
    {
      return;
    }

    _cameraController.OnCameraModifierEnter(_cameraMovementSettings);
  }

  void OnExitTriggerInvoked(object sender, TriggerEnterExitEventArgs e)
  {
    _cameraController.OnCameraModifierExit(_cameraMovementSettings);
  }
}
