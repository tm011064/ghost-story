using UnityEngine;

public partial class VerticalGroundSnappingCalculator : ICameraPositionCalculator
{
  private readonly CameraController _cameraController;

  private readonly PlayerController _player;

  private readonly CameraMovementSettings _cameraMovementSettings;

  private readonly float _screenHeight;

  private readonly float _screenCenter;

  private readonly float _topVerticalLockPosition;

  private readonly float _bottomVerticalLockPosition;

  private readonly SmoothDampedPositionCalculator _smoothDampedPositionCalculator = new SmoothDampedPositionCalculator();

  private CameraPositionCalculationResult _lastResult;

  private float _smoothDampVelocity;

  private float _cameraPosition;

  public VerticalGroundSnappingCalculator(
    CameraMovementSettings cameraMovementSettings,
    CameraController cameraController,
    PlayerController player)
  {
    _screenHeight = cameraController.TargetScreenSize.y;
    _screenCenter = _screenHeight * .5f / cameraMovementSettings.ZoomSettings.ZoomPercentage;
    _topVerticalLockPosition = cameraMovementSettings.VerticalLockSettings.TopVerticalLockPosition - _screenCenter;
    _bottomVerticalLockPosition = cameraMovementSettings.VerticalLockSettings.BottomVerticalLockPosition + _screenCenter;

    _cameraMovementSettings = cameraMovementSettings;
    _cameraController = cameraController;
    _player = player;

    var adjustedCameraPosition = Mathf.Clamp(player.transform.position.y, _bottomVerticalLockPosition, _topVerticalLockPosition);
    _lastResult = new CameraPositionCalculationResult
    {
      LastCameraPosition = adjustedCameraPosition,
      CameraPosition = adjustedCameraPosition,
      LastWindowPosition = cameraController.transform.position.y,
      WindowPosition = cameraController.transform.position.y
    };
    _cameraPosition = _lastResult.CameraPosition;
  }

  public float GetCameraPosition()
  {
    return _cameraPosition;
  }

  public void Update()
  {
    _lastResult = CalculateVerticalPosition();

    _cameraPosition = _smoothDampedPositionCalculator.CalculatePosition(
      _cameraController.transform.position.y,
      _lastResult.CameraPosition,
      _lastResult.CameraSmoothDampSpeed == CameraSmoothDampSpeed.Fast
        ? _cameraMovementSettings.SmoothDampMoveSettings.VerticalFastSmoothDampTime
        : _cameraMovementSettings.SmoothDampMoveSettings.VerticalSlowSmoothDampTime);
  }

  public float CalculateTargetPosition()
  {
    var result = CalculateVerticalPosition();

    return result.CameraPosition;
  }

  private CameraPositionCalculationResult CalculateVerticalPosition()
  {
    if (_player.IsAirborne())
    {
      if (_player.IsGoingUp())
      {
        return CalculateGoingUp();
      }

      if (_player.IsGoingDown())
      {
        return CalculateGoingDown();
      }

      return _lastResult;
    }

    if (_player.GotGroundedThisFrame())
    {
      return CalculateGroundedThisFrame();
    }

    return _lastResult;
  }

  private CameraPositionCalculationResult CalculateGroundedThisFrame()
  {
    if (_player.transform.position.y > CalculateInnerWindowTopBoundsPosition())
    {
      var position = _player.transform.position.y + CalculateUpwardMovementSnapPosition();

      return CreateNextResult(position, position, CameraSmoothDampSpeed.Slow);
    }

    if (_player.transform.position.y < CalculateInnerWindowBottomBoundsPosition())
    {
      var position = _player.transform.position.y + CalculateDownwardMovementSnapPosition();

      return CreateNextResult(position, position, CameraSmoothDampSpeed.Slow);
    }

    _lastResult.CameraSmoothDampSpeed = CameraSmoothDampSpeed.Slow;
    return _lastResult;
  }

  private CameraPositionCalculationResult CalculateGoingUp()
  {
    var outerWindowTopDelta = _player.transform.position.y - CalculateOuterWindowTopBoundsPosition();
    if (outerWindowTopDelta > 0)
    {
      var cameraPosition = _cameraController.transform.position.y + outerWindowTopDelta;
      return CreateNextResult(
        cameraPosition,
        _lastResult.WindowPosition,
        CameraSmoothDampSpeed.Slow);
    }

    return _lastResult;
  }

  private CameraPositionCalculationResult CalculateGoingDown()
  {
    var outerWindowTopDelta = _player.transform.position.y - CalculateOuterWindowTopBoundsPosition();
    if (outerWindowTopDelta > 0)
    {
      var cameraPosition = _cameraController.transform.position.y + outerWindowTopDelta;
      return CreateNextResult(
        cameraPosition,
        _lastResult.WindowPosition,
        CameraSmoothDampSpeed.Slow);
    }

    var bottomBoundary = Mathf.Min(
      CalculateOuterWindowBottomBoundsPosition(),
      CalculateInnerWindowBottomBoundsPosition());

    var outerWindowBottomDelta = _player.transform.position.y - bottomBoundary;

    if (outerWindowBottomDelta < 0)
    {
      var cameraPosition = _cameraController.transform.position.y + outerWindowBottomDelta;

      return CreateNextResult(
        cameraPosition,
        _lastResult.WindowPosition,
        CameraSmoothDampSpeed.Fast);
    }

    return _lastResult;
  }

  private float CalculateInnerWindowTopBoundsPosition()
  {
    return
      _lastResult.WindowPosition
      + (_screenHeight
        * _cameraMovementSettings.VerticalSnapWindowSettings.InnerWindowTopBoundsPositionPercentage
        / _cameraMovementSettings.ZoomSettings.ZoomPercentage)
      - _screenCenter;
  }

  private float CalculateInnerWindowBottomBoundsPosition()
  {
    return
      _lastResult.WindowPosition
      + (_screenHeight
        * _cameraMovementSettings.VerticalSnapWindowSettings.InnerWindowBottomBoundsPositionPercentage
        / _cameraMovementSettings.ZoomSettings.ZoomPercentage)
      - _screenCenter;
  }

  private float CalculateOuterWindowTopBoundsPosition()
  {
    return
      _cameraController.transform.position.y
      + (_screenHeight
        * _cameraMovementSettings.VerticalSnapWindowSettings.OuterWindowTopBoundsPositionPercentage
        / _cameraMovementSettings.ZoomSettings.ZoomPercentage)
      - _screenCenter;
  }

  private float CalculateOuterWindowBottomBoundsPosition()
  {
    return
      _cameraController.transform.position.y
      + (_screenHeight
        * _cameraMovementSettings.VerticalSnapWindowSettings.OuterWindowBottomBoundsPositionPercentage
        / _cameraMovementSettings.ZoomSettings.ZoomPercentage)
      - _screenCenter;
  }

  private float CalculateUpwardMovementSnapPosition()
  {
    return
      _screenHeight
      * _cameraMovementSettings.VerticalSnapWindowSettings.UpwardMovementSnapPositionPercentage
      / _cameraMovementSettings.ZoomSettings.ZoomPercentage;
  }

  private float CalculateDownwardMovementSnapPosition()
  {
    return
      _screenHeight
      * _cameraMovementSettings.VerticalSnapWindowSettings.DownwardMovementSnapPositionPercentage
      / _cameraMovementSettings.ZoomSettings.ZoomPercentage;
  }

  private CameraPositionCalculationResult CreateNextResult(
    float cameraPosition,
    float windowPosition,
    CameraSmoothDampSpeed cameraSmoothDampSpeed)
  {
    var adjustedCameraPosition = Mathf.Clamp(
      cameraPosition,
      _bottomVerticalLockPosition,
      _topVerticalLockPosition);

    return new CameraPositionCalculationResult
    {
      LastCameraPosition = _lastResult.CameraPosition,
      CameraPosition = adjustedCameraPosition,
      LastWindowPosition = _lastResult.WindowPosition,
      WindowPosition = windowPosition,
      CameraSmoothDampSpeed = cameraSmoothDampSpeed
    };
  }

  private class CameraPositionCalculationResult
  {
    public float WindowPosition;

    public float LastWindowPosition;

    public float CameraPosition;

    public CameraSmoothDampSpeed CameraSmoothDampSpeed;

    public float LastCameraPosition;
  }

  private enum CameraSmoothDampSpeed
  {
    Fast,
    Slow
  }
}
