using UnityEngine;

public partial class HorizontalWindowCameraPositionCalculator : ICameraPositionCalculator
{
  private readonly CameraController _cameraController;

  private readonly PlayerController _player;

  private readonly CameraMovementSettings _cameraMovementSettings;

  private readonly float _leftVerticalLockPosition;

  private readonly float _rightVerticalLockPosition;

  private float _smoothDampVelocity;

  private float _cameraPosition;

  private float _windowPosition;

  private HorizontalDirection _lastDirection;

  public HorizontalWindowCameraPositionCalculator(
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
    var targetPosition = Mathf.Clamp(CalculateUpdatePosition(), _leftVerticalLockPosition, _rightVerticalLockPosition);

    _cameraPosition = CalculateCameraPosition(targetPosition);
  }

  private float CalculateCameraPosition(float targetPosition)
  {
    //if (_smoothDampVelocity > 0)
    if (_lastDirection == HorizontalDirection.Right)
    {
      return Mathf.SmoothDamp(
        _cameraController.transform.position.x,
        targetPosition,
        ref _smoothDampVelocity,
        _cameraMovementSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime);
    }

    var smoothDampValue = Mathf.SmoothDamp(
      0,
      _cameraController.transform.position.x - targetPosition,
      ref _smoothDampVelocity,
      _cameraMovementSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime);

    return _cameraPosition - smoothDampValue;
  }

  private float CalculateUpdatePosition()
  {
    if (!IsPlayerMoving())
    {
      return _cameraController.transform.position.x;
    }

    UpdateDirection();

    UpdateWindowPosition();

    return _player.transform.position.x + _windowPosition;
  }

  private void UpdateWindowPosition()
  {
    var targetWindowPosition = _windowPosition
      + (_player.CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.x
        * _cameraMovementSettings.HorizontalCamereaWindowSettings.DirectionChangeDamping);

    _windowPosition = Mathf.Clamp(
      targetWindowPosition,
      -_cameraMovementSettings.HorizontalCamereaWindowSettings.WindowWitdh,
      _cameraMovementSettings.HorizontalCamereaWindowSettings.WindowWitdh);
  }

  private void UpdateDirection()
  {
    var currentDirection = _player.CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.x > 0
      ? HorizontalDirection.Right
      : HorizontalDirection.Left;

    if (currentDirection != _lastDirection)
    {
      _windowPosition = 0;
      _smoothDampVelocity = 0;
    }

    _lastDirection = currentDirection;
  }

  private bool IsPlayerMoving()
  {
    return !Mathf.Approximately(_player.CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.x, 0);
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
