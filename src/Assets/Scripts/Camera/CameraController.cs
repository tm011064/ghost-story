using System;
using UnityEngine;

public partial class CameraController : MonoBehaviour
{
  public Vector3 CameraOffset; // TODO (Roman): only z is used, so this could be a float

  public bool UseFixedUpdate = false;

  public Vector2 TargetScreenSize = new Vector2(1920, 1080);

  public bool UseFixedAspectRatio = false;

  public Vector2 FixedAspectRatio = new Vector2(4, 3);

  [HideInInspector]
  public Transform Target;

  [HideInInspector]
  public Transform Transform;

  [HideInInspector]
  public CameraTrolley[] CameraTrolleys;

  [HideInInspector]
  public GameManager GameManager;

  [HideInInspector]
  public Vector3 LastTargetPosition;

  private readonly CameraMovementSettingsManager _cameraMovementSettingsManager = new CameraMovementSettingsManager();
  
  private UpdateTimer _zoomTimer;

  private ICameraPositionCalculator _verticalCameraPositionCalculator;

  private ICameraPositionCalculator _horizontalCameraPositionCalculator;

  private readonly TranslateTransformActionsManager _scrollActionManager = new TranslateTransformActionsManager();

  public event Action<TranslateTransformActions> ScrollActionCompleted;

  public void RegisterScrollActions(TranslateTransformActions scrollActions)
  {
    _scrollActionManager.EnqueueScrollActions(scrollActions);
  }

  public void MoveCameraToTargetPosition()
  {
    SetPosition(Target.position);

    var calculatedPosition = CalculateTargetPosition(_cameraMovementSettingsManager.ActiveSettings);

    SetPosition(calculatedPosition);
  }

  public void SetPosition(Vector3 position)
  {
    transform.position = position;

    LastTargetPosition = position;
  }

  public Vector2 GetScreenSize()
  {
    var defaultOrthographicSize = (TargetScreenSize.y * .5f);

    var zoomPercentage = Camera.main.orthographicSize / defaultOrthographicSize;

    return new Vector2(
      (float)TargetScreenSize.x * zoomPercentage,
      (float)TargetScreenSize.y * zoomPercentage);
  }

  public bool IsPointVisible(Vector2 point)
  {
    var screenSize = GetScreenSize();

    var rect = new Rect(
      Camera.main.transform.position.x - screenSize.x * .5f,
      Camera.main.transform.position.y - screenSize.y * .5f,
      screenSize.x,
      screenSize.y);

    return rect.Contains(point);
  }

  public bool IsCurrentCameraMovementSettings(CameraMovementSettings cameraMovementSettings)
  {
    return _cameraMovementSettingsManager.ActiveSettings.Equals(cameraMovementSettings);
  }

  public void OnCameraModifierEnter(CameraMovementSettings cameraMovementSettings)
  {
    _cameraMovementSettingsManager.AddSettings(cameraMovementSettings);
  }

  public void Reset()
  {
    _cameraMovementSettingsManager.ClearSettings();
  }

  public void OnCameraModifierExit(CameraMovementSettings cameraMovementSettings)
  {
    _cameraMovementSettingsManager.RemoveSettings(cameraMovementSettings);
  }

  void OnCameraMovementSettingsChanged(CameraSettingsChangedArguments args)
  {
    _verticalCameraPositionCalculator = CreateVerticalPositionCalculator(_cameraMovementSettingsManager.ActiveSettings);
    _horizontalCameraPositionCalculator = CreateHorizontalPositionCalculator(_cameraMovementSettingsManager.ActiveSettings);

    CameraOffset = new Vector3(
      _cameraMovementSettingsManager.ActiveSettings.Offset.x,
      _cameraMovementSettingsManager.ActiveSettings.Offset.y,
      CameraOffset.z);

    var targetOrthographicSize = (TargetScreenSize.y * .5f) / _cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomPercentage;

    if (!Mathf.Approximately(Camera.main.orthographicSize, targetOrthographicSize))
    {
      Logger.Info("Start zoom to target size: " + targetOrthographicSize + ", current size: " + Camera.main.orthographicSize);

      if (_cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomTime == 0f)
      {
        Camera.main.orthographicSize = targetOrthographicSize;
      }
      else
      {
        _zoomTimer = new ZoomTimer(_cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomTime, Camera.main.orthographicSize, targetOrthographicSize, _cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomEasingType);

        _zoomTimer.Start();
      }
    }

    Logger.Info("Camera movement set to: " + _cameraMovementSettingsManager.ActiveSettings.ToString());
    Logger.Info("Camera size; current: " + Camera.main.orthographicSize + ", target: " + targetOrthographicSize);

    if (args.SettingsWereRefreshed)
    {
      MoveCameraToTargetPosition();
    }
  }

  void Awake()
  {
    _cameraMovementSettingsManager.SettingsChanged += OnCameraMovementSettingsChanged;
    _scrollActionManager.Completed += OnScrollActionManagerCompleted;
  }

  void OnDestroy()
  {
    _cameraMovementSettingsManager.SettingsChanged -= OnCameraMovementSettingsChanged;
    _scrollActionManager.Completed -= OnScrollActionManagerCompleted;
  }

  void OnScrollActionManagerCompleted(TranslateTransformActions translateTransformActions)
  {
    var handler = ScrollActionCompleted;
    if (handler != null)
    {
      handler(translateTransformActions);
    }
  }

  void Start()
  {
    if (UseFixedAspectRatio)
    {
      AssignFixedAspectRatio(FixedAspectRatio.x / FixedAspectRatio.y);
    }

    if (CameraTrolleys == null)
    {
      CameraTrolleys = FindObjectsOfType<CameraTrolley>();

      Debug.Log("Found " + CameraTrolleys.Length + " camera trolleys.");
    }

    Transform = gameObject.transform;

    GameManager = GameManager.Instance;

    Target = GameManager.Player.transform;

    LastTargetPosition = Target.transform.position;

    Logger.Info("Window size: " + Screen.width + " x " + Screen.height);
  }

  void Update()
  {
    if (_zoomTimer != null)
    {
      _zoomTimer.Update();

      if (_zoomTimer.HasEnded)
      {
        _zoomTimer = null;
      }
    }
  }

  void LateUpdate()
  {
    if (!UseFixedUpdate)
    {
      UpdateCameraPosition();
    }
  }

  void FixedUpdate()
  {
    if (UseFixedUpdate)
    {
      UpdateCameraPosition();
    }
  }

  void UpdateCameraPosition()
  {
    if (IsCameraControlledByScrollAction())
    {
      return;
    }

    if (_cameraMovementSettingsManager.ActiveSettings == null)
    {
      return;
    }

    _verticalCameraPositionCalculator.Update();
    _horizontalCameraPositionCalculator.Update();

    //TargetedTransformPositionX = _horizontalCameraPositionCalculator.CalculateTargetPosition(); // TODO (Roman): this can be removed

    transform.position = new Vector3(
      _horizontalCameraPositionCalculator.GetCameraPosition(),
      _verticalCameraPositionCalculator.GetCameraPosition(),
      Target.position.z - CameraOffset.z);

    LastTargetPosition = Target.transform.position;
  }

  // TODO (Roman): move 
  private ICameraPositionCalculator CreateVerticalPositionCalculator(CameraMovementSettings cameraMovementSettings)
  {
    switch (cameraMovementSettings.CameraSettings.VerticalCameraFollowMode)
    {
      case VerticalCameraFollowMode.FollowWhenGrounded:
        return new VerticalGroundSnappingCalculator(
          cameraMovementSettings,
          this,
          GameManager.Instance.Player);

      case VerticalCameraFollowMode.FollowAlways:
        return new VerticalFollowPlayerCameraPositionCalculator(
          cameraMovementSettings,
          this,
          GameManager.Instance.Player);
    }

    throw new NotSupportedException();
  }
  private ICameraPositionCalculator CreateHorizontalPositionCalculator(CameraMovementSettings cameraMovementSettings)
  {
    switch (cameraMovementSettings.CameraSettings.HorizontalCameraFollowMode)
    {
      case HorizontalCameraFollowMode.FollowAlways:
        return new HorizontalFollowPlayerCameraPositionCalculator(
          cameraMovementSettings,
          this,
          GameManager.Instance.Player);
    }

    throw new NotSupportedException();
  }

  private void AssignFixedAspectRatio(float targetAspectRatio)
  {
    var windowaspect = (float)Screen.width / (float)Screen.height;

    var scaleheight = windowaspect / targetAspectRatio;

    var camera = GetComponent<Camera>();

    if (scaleheight < 1.0f) // add letterbox
    {
      var rect = camera.rect;

      rect.width = 1.0f;
      rect.height = scaleheight;
      rect.x = 0;
      rect.y = (1.0f - scaleheight) / 2.0f;

      camera.rect = rect;
    }
    else // add pillarbox
    {
      var scalewidth = 1.0f / scaleheight;

      var rect = camera.rect;

      rect.width = scalewidth;
      rect.height = 1.0f;
      rect.x = (1.0f - scalewidth) / 2.0f;
      rect.y = 0;

      camera.rect = rect;
    }
  }

  public Vector3 CalculateTargetPosition(CameraMovementSettings cameraMovementSettings)
  {
    var verticalCalculator = CreateVerticalPositionCalculator(cameraMovementSettings);
    var horizontalCalculator = CreateHorizontalPositionCalculator(cameraMovementSettings);

    return new Vector3(
      horizontalCalculator.CalculateTargetPosition(),
      verticalCalculator.CalculateTargetPosition(),
      Target.position.z - CameraOffset.z);
  }

  private bool IsCameraControlledByScrollAction()
  {
    _scrollActionManager.Update(transform.position);

    if (!_scrollActionManager.HasActions())
    {
      return false;
    }

    transform.position = _scrollActionManager.GetPosition();

    return true;
  }

  class ZoomTimer : UpdateTimer
  {
    private float _startSize;

    private float _targetSize;

    private EasingType _easingType;

    public ZoomTimer(float duration, float startSize, float targetSize, EasingType easingType)
      : base(duration)
    {
      _startSize = startSize;
      _targetSize = targetSize;
      _easingType = easingType;
    }

    protected override void DoUpdate(float currentTime)
    {
      var value = Easing.GetValue(_easingType, currentTime, _duration);

      Camera.main.orthographicSize = _startSize + (_targetSize - _startSize) * value;
    }
  }
}
