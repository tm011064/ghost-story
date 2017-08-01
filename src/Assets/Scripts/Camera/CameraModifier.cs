using UnityEngine;

public partial class CameraModifier : CameraMovementSettingsBehaviour, ICameraModifier
{
  public VerticalLockSettings VerticalLockSettings;

  public HorizontalLockSettings HorizontalLockSettings;

  public Color GizmoColor = Color.magenta;

  public bool MustBeOnLadderToEnter;

  private CameraController _cameraController;

  void Awake()
  {
    _cameraController = Camera.main.GetComponent<CameraController>();
    CameraMovementSettings = CreateCameraMovementSettings();

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
    _cameraController.OnCameraModifierEnter(CameraMovementSettings);
  }

  protected virtual void OnAwake()
  {
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
    CameraMovementSettings = CreateCameraMovementSettings();

    var playerPosition = GameManager.Instance.Player.transform.position;
    if (CameraMovementSettings.Contains(playerPosition))
    {
      _cameraController.OnCameraModifierEnter(CameraMovementSettings);
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

    _cameraController.OnCameraModifierEnter(CameraMovementSettings);
  }

  void OnExitTriggerInvoked(object sender, TriggerEnterExitEventArgs e)
  {
    _cameraController.OnCameraModifierExit(CameraMovementSettings);
  }

  protected override VerticalLockSettings CreateBaseVerticalLockSettings()
  {
    return VerticalLockSettings;
  }

  protected override HorizontalLockSettings CreateBaseHorizontalLockSettings()
  {
    return HorizontalLockSettings;
  }
}
