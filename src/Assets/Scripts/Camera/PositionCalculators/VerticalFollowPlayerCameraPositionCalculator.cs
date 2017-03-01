using UnityEngine;

public partial class VerticalFollowPlayerCameraPositionCalculator : ICameraPositionCalculator
{
  private readonly CameraController _cameraController;

  private readonly PlayerController _player;

  private readonly CameraMovementSettings _cameraMovementSettings;

  private readonly float _topVerticalLockPosition;

  private readonly float _bottomVerticalLockPosition;

  private readonly SmoothDampedPositionCalculator _smoothDampedPositionCalculator;

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

    _smoothDampedPositionCalculator = new SmoothDampedPositionCalculator(
      player.CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.y);

    _cameraPosition = Mathf.Clamp(player.transform.position.y, _bottomVerticalLockPosition, _topVerticalLockPosition);
  }

  public float WindowPosition { get { return 0f; } }

  public SmoothDampedPositionCalculator SmoothDampedPositionCalculator { get { return _smoothDampedPositionCalculator; } }

  public void Update()
  {
    var position = Mathf.Clamp(_player.transform.position.y, _bottomVerticalLockPosition, _topVerticalLockPosition);

    _cameraPosition = _smoothDampedPositionCalculator.CalculatePosition(
      _cameraController.transform.position.y,
      position,
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
