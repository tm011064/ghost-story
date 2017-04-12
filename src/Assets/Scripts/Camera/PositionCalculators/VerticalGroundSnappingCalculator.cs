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

  private readonly SmoothDampedPositionCalculator _smoothDampedPositionCalculator;

  private readonly SlopeCalculator _slopeCalculator;

  private CameraPositionCalculationResult _lastResult;

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

    _smoothDampedPositionCalculator = cameraController.VerticalCameraPositionCalculator != null
      ? cameraController.VerticalCameraPositionCalculator.SmoothDampedPositionCalculator.Clone()
      : new SmoothDampedPositionCalculator(player.CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.y);

    _lastResult = CreateLastResultFromCameraController();

    _cameraPosition = _cameraController.transform.position.y;
    _slopeCalculator = new SlopeCalculator(this);
  }

  public float WindowPosition { get { return _lastResult.WindowPosition; } }

  public SmoothDampedPositionCalculator SmoothDampedPositionCalculator { get { return _smoothDampedPositionCalculator; } }

  private CameraPositionCalculationResult CreateLastResultFromCameraController()
  {
    if (_cameraController.VerticalCameraPositionCalculator == null)
    {
      var adjustedCameraPosition = Mathf.Clamp(_player.transform.position.y, _bottomVerticalLockPosition, _topVerticalLockPosition);
      return new CameraPositionCalculationResult
      {
        LastCameraPosition = adjustedCameraPosition,
        CameraPosition = adjustedCameraPosition,
        LastWindowPosition = adjustedCameraPosition,
        WindowPosition = adjustedCameraPosition
      };
    }

    var verticalGroundSnappingCalculator = _cameraController.VerticalCameraPositionCalculator as VerticalGroundSnappingCalculator;
    if (verticalGroundSnappingCalculator != null)
    {
      return verticalGroundSnappingCalculator._lastResult;
    }

    return new CameraPositionCalculationResult
    {
      LastCameraPosition = _cameraController.transform.position.y,
      CameraPosition = _cameraController.transform.position.y,
      LastWindowPosition = _cameraController.VerticalCameraPositionCalculator.WindowPosition,
      WindowPosition = _cameraController.VerticalCameraPositionCalculator.WindowPosition
    };
  }

  public float GetCameraPosition()
  {
    return _cameraPosition;
  }

  public void Update()
  {
    _slopeCalculator.Update();
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
    if (_player.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.IsOnSlope)
    {
      return _slopeCalculator.Calculate();
    }

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

  private class SlopeCalculator
  {
    private readonly VerticalGroundSnappingCalculator _calculator;

    private bool _onSlope;

    private float _targetPosition;

    private float _deltaTargetPosition;

    public SlopeCalculator(VerticalGroundSnappingCalculator calculator)
    {
      _calculator = calculator;
    }

    public void Update()
    {
      if (!_calculator._player.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.IsOnSlope)
      {
        _onSlope = false;

        return;
      }

      if (!_onSlope || _calculator._player.CharacterPhysicsManager.LastMoveCalculationResult.HasHorizontalDirectionChanged())
      {
        _targetPosition = CalculateTargetPosition();
        _deltaTargetPosition = _calculator._lastResult.CameraPosition - _targetPosition;
      }

      _onSlope = true;
    }

    private float CalculateTargetPosition()
    {
      if (_calculator._player.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.BecameGroundedThisFrame)
      {
        return _calculator._lastResult.CameraPosition;
      }

      var snapPosition = _calculator._player.CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.FacingUpSlope
        ? _calculator.CalculateUpwardMovementSnapPosition()
        : _calculator.CalculateDownwardMovementSnapPosition();

      return _calculator._player.transform.position.y + snapPosition;
    }

    public CameraPositionCalculationResult Calculate()
    {
      _deltaTargetPosition = Mathf.Lerp(_deltaTargetPosition, 0, Time.deltaTime * 4f);

      if (_calculator._player.CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.x != 0)
      {
        _targetPosition = CalculateTargetPosition();
      }

      var position = _targetPosition + _deltaTargetPosition;

      return _calculator.CreateNextResult(position, position, CameraSmoothDampSpeed.Slow);
    }
  }

  private class CameraPositionCalculationResult
  {
    public float WindowPosition;

    public float LastWindowPosition;

    public float CameraPosition;

    public CameraSmoothDampSpeed CameraSmoothDampSpeed;

    public float LastCameraPosition;

    public override string ToString()
    {
      return this.GetFieldValuesFormatted();
    }
  }

  private enum CameraSmoothDampSpeed
  {
    Fast,
    Slow
  }
}
