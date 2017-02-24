using UnityEngine;

public partial class VerticalFollowPlayerCameraPositionCalculator : ICameraPositionCalculator
{
  private readonly CameraController _cameraController;

  private readonly PlayerController _player;

  private readonly CameraMovementSettings _cameraMovementSettings;

  private readonly float _topVerticalLockPosition;

  private readonly float _bottomVerticalLockPosition;

  private float _smoothDampVelocity;

  private float _cameraPosition;

  public VerticalFollowPlayerCameraPositionCalculator(
    CameraMovementSettings cameraMovementSettings,
    CameraController cameraController,
    PlayerController player)
  {
    var screenHeight = cameraController.TargetScreenSize.y;
    var screenCenter = screenHeight * .5f / cameraMovementSettings.ZoomSettings.ZoomPercentage;
    _topVerticalLockPosition = cameraMovementSettings.VerticalLockSettings.TopVerticalLockPosition - screenCenter;
    _bottomVerticalLockPosition = cameraMovementSettings.VerticalLockSettings.BottomVerticalLockPosition + screenCenter;

    _cameraMovementSettings = cameraMovementSettings;
    _cameraController = cameraController;
    _player = player;

    _cameraPosition = Mathf.Clamp(player.transform.position.y, _bottomVerticalLockPosition, _topVerticalLockPosition);
  }

  public void Update()
  {
    var position = Mathf.Clamp(_player.transform.position.y, _bottomVerticalLockPosition, _topVerticalLockPosition);

    _cameraPosition = Mathf.SmoothDamp(
        _cameraController.transform.position.y,
        position,
        ref _smoothDampVelocity,
        _cameraMovementSettings.SmoothDampMoveSettings.VerticalSlowSmoothDampTime);
  }

  public float CalculateTargetPosition()
  {
    return Mathf.Clamp(_player.transform.position.y, _bottomVerticalLockPosition, _topVerticalLockPosition);
  }

  public float GetCameraPosition()
  {
    return _cameraPosition;
  }
}
