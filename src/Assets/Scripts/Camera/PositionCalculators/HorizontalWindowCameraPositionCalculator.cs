using UnityEngine;

public partial class HorizontalWindowCameraPositionCalculator : ICameraPositionCalculator
{
  private readonly CameraController _cameraController;

  private readonly PlayerController _player;

  private readonly CameraMovementSettings _cameraMovementSettings;

  private readonly float _leftVerticalLockPosition;

  private readonly float _leftVerticalLockPositionWithFudgeFactor;

  private readonly float _rightVerticalLockPosition;

  private readonly float _rightVerticalLockPositionWithFudgeFactor;

  private readonly SmoothDampedPositionCalculator _smoothDampedPositionCalculator;

  private float _cameraPosition;

  private float _windowPosition;

  public HorizontalWindowCameraPositionCalculator(
    CameraMovementSettings cameraMovementSettings,
    CameraController cameraController,
    PlayerController player)
  {
    var screenCenter = cameraController.TargetScreenSize.x * .5f / cameraMovementSettings.ZoomSettings.ZoomPercentage;

    var fudgeFactor = 1f;
    _leftVerticalLockPosition = cameraMovementSettings.HorizontalLockSettings.LeftHorizontalLockPosition + screenCenter;
    _leftVerticalLockPositionWithFudgeFactor = _leftVerticalLockPosition - fudgeFactor;
    _rightVerticalLockPosition = cameraMovementSettings.HorizontalLockSettings.RightHorizontalLockPosition - screenCenter;
    _rightVerticalLockPositionWithFudgeFactor = _rightVerticalLockPosition + fudgeFactor;

    _cameraMovementSettings = cameraMovementSettings;
    _cameraController = cameraController;
    _player = player;

    if (cameraController.HorizontalCameraPositionCalculator != null)
    {
      _windowPosition = cameraController.HorizontalCameraPositionCalculator.WindowPosition;
      _smoothDampedPositionCalculator = cameraController.HorizontalCameraPositionCalculator.SmoothDampedPositionCalculator.Clone();
    }
    else
    {
      _smoothDampedPositionCalculator = new SmoothDampedPositionCalculator(player.CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.x);
    }

    _cameraPosition = _cameraController.transform.position.x;
  }

  public float WindowPosition { get { return _windowPosition; } }

  public SmoothDampedPositionCalculator SmoothDampedPositionCalculator { get { return _smoothDampedPositionCalculator; } }

  public void Update()
  {
    var updatedPosition = CalculateUpdatePosition();

    var targetPosition = AdjustLocks(updatedPosition);

    _cameraPosition = _smoothDampedPositionCalculator.CalculatePosition(
      _cameraController.transform.position.x,
      targetPosition,
      _cameraMovementSettings.SmoothDampMoveSettings.HorizontalSmoothDampTime);
  }

  private float AdjustLocks(float position)
  {
    if (!IsPlayerMoving())
    {
      return position;
    }

    if (position < _leftVerticalLockPosition
      && _cameraPosition >= _leftVerticalLockPositionWithFudgeFactor)
    {
      return _leftVerticalLockPosition;
    }

    if (position > _rightVerticalLockPosition
      && _cameraPosition <= _rightVerticalLockPositionWithFudgeFactor)
    {
      return _rightVerticalLockPosition;
    }

    return position;
  }

  private float CalculateUpdatePosition()
  {
    if (!IsPlayerMoving())
    {
      return _cameraController.transform.position.x;
    }

    if (_player.CharacterPhysicsManager.LastMoveCalculationResult.HasHorizontalDirectionChanged())
    {
      _windowPosition = 0;
    }

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
