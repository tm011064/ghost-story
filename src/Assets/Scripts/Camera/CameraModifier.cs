using System;
using UnityEngine;

public partial class CameraModifier : MonoBehaviour
{
  public VerticalLockSettings VerticalLockSettings;

  public HorizontalLockSettings HorizontalLockSettings;

  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  public CameraSettings CameraSettings;

  [Tooltip("The (x, y) offset of the camera. This can be used when default vertical locking is disabled and you want the player to be below, above, right or left of the screen center.")]
  public Vector2 Offset;

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

  protected void OverrideSettings(SmoothDampMoveSettings smoothDampMoveSettings, CameraSettings cameraSettings)
  {
    CameraSettings = cameraSettings;
    SmoothDampMoveSettings = smoothDampMoveSettings;

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
      VerticalLockSettings,
      HorizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      Offset,
      CameraSettings);
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
