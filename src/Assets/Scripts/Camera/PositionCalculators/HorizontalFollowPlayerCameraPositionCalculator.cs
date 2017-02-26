using UnityEngine;

public partial class HorizontalFollowPlayerCameraPositionCalculator : ICameraPositionCalculator
{
  private readonly CameraController _cameraController;

  private readonly PlayerController _player;

  private readonly CameraMovementSettings _cameraMovementSettings;

  private readonly SmoothDampedPositionCalculator _smoothDampedPositionCalculator = new SmoothDampedPositionCalculator();

  private readonly float _leftVerticalLockPosition;

  private readonly float _rightVerticalLockPosition;

  private float _smoothDampVelocity;

  private float _cameraPosition;

  public HorizontalFollowPlayerCameraPositionCalculator(
    CameraMovementSettings cameraMovementSettings,
    CameraController cameraController,
    PlayerController player)
  {
    var screenCenter = cameraController.TargetScreenSize.x * .5f / cameraMovementSettings.ZoomSettings.ZoomPercentage;

    _leftVerticalLockPosition = cameraMovementSettings.HorizontalLockSettings.LeftHorizontalLockPosition + screenCenter;
    _rightVerticalLockPosition = cameraMovementSettings.HorizontalLockSettings.RightHorizontalLockPosition - screenCenter;

    _cameraMovementSettings = cameraMovementSettings;
    _cameraController = cameraController;
    _player = player;

    _cameraPosition = Mathf.Clamp(player.transform.position.x, _leftVerticalLockPosition, _rightVerticalLockPosition);
  }

  public void Update()
  {
    var position = Mathf.Clamp(_player.transform.position.x, _leftVerticalLockPosition, _rightVerticalLockPosition);

    _cameraPosition = _smoothDampedPositionCalculator.CalculatePosition(
      _cameraController.transform.position.x,
      position,
      _cameraMovementSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime);
  }

  public float CalculateTargetPosition()
  {
    return Mathf.Clamp(_player.transform.position.x, _leftVerticalLockPosition, _rightVerticalLockPosition);
  }

  public float GetCameraPosition()
  {
    return _cameraPosition;
  }
}
