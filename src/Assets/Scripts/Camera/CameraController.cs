using System;
using UnityEngine;

public partial class CameraController : MonoBehaviour
{
  public float ZAxisOffset;

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

  public ICameraPositionCalculator VerticalCameraPositionCalculator { get { return _verticalCameraPositionCalculator; } }

  public ICameraPositionCalculator HorizontalCameraPositionCalculator { get { return _horizontalCameraPositionCalculator; } }

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
    _verticalCameraPositionCalculator = null;
    _horizontalCameraPositionCalculator = null;
  }

  public void OnCameraModifierExit(CameraMovementSettings cameraMovementSettings)
  {
    _cameraMovementSettingsManager.RemoveSettings(cameraMovementSettings);
  }

  void OnCameraMovementSettingsChanged()
  {
    _verticalCameraPositionCalculator = CamerPositionCalculatorFactory.CreateVertical(
      this,
      _cameraMovementSettingsManager.ActiveSettings);
    _horizontalCameraPositionCalculator = CamerPositionCalculatorFactory.CreateHorizontal(
      this,
      _cameraMovementSettingsManager.ActiveSettings);

    SetCameraSize();

    // TODO (Roman): don't move player on scene load only, otherwise do
    // TODO (Roman): freeze player longer after finish - quick back kills door scroll
    if (_cameraMovementSettingsManager.SettingsCount == 1
      && !GameManager.Instance.SceneManager.IsLoading()) // TODO (Roman): the isloading is bad design
    {
      MoveCameraToTargetPosition();
    }

    Logger.Info("Camera movement set to: " + _cameraMovementSettingsManager.ActiveSettings.ToString());
  }

  private void SetCameraSize()
  {
    var targetOrthographicSize = (TargetScreenSize.y * .5f) / _cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomPercentage;

    if (Mathf.Approximately(Camera.main.orthographicSize, targetOrthographicSize))
    {
      Logger.Info("Camera size remains at: " + Camera.main.orthographicSize);

      return;
    }

    if (_cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomTime == 0f)
    {
      Logger.Info("Camera size set to: " + targetOrthographicSize);

      Camera.main.orthographicSize = targetOrthographicSize;

      return;
    }

    Logger.Info("Start camera zoom from " + Camera.main.orthographicSize + " to " + targetOrthographicSize);

    _zoomTimer = new ZoomTimer(
      _cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomTime,
      Camera.main.orthographicSize,
      targetOrthographicSize,
      _cameraMovementSettingsManager.ActiveSettings.ZoomSettings.ZoomEasingType);

    _zoomTimer.Start();
  }

  void Awake()
  {
    _cameraMovementSettingsManager.SettingsChanged += OnCameraMovementSettingsChanged;
    _scrollActionManager.Completed += OnScrollActionManagerCompleted;
    Transform = gameObject.transform;
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

    transform.position = new Vector3(
      _horizontalCameraPositionCalculator.GetCameraPosition(),
      _verticalCameraPositionCalculator.GetCameraPosition(),
      Target.position.z - ZAxisOffset);

    LastTargetPosition = Target.transform.position;
  }

  private void AssignFixedAspectRatio(float targetAspectRatio)
  {
    var screenAspectRatio = (float)Screen.width / (float)Screen.height;

    var scaleHeight = screenAspectRatio / targetAspectRatio;

    if (NeedsLetterBox(scaleHeight))
    {
      SetCameraLetterBoxRect(scaleHeight);
      return;
    }

    SetCameraPillarBoxRect(scaleHeight);
  }

  private void SetCameraPillarBoxRect(float scaleHeight)
  {
    var camera = GetComponent<Camera>();
    var scaleWidth = 1f / scaleHeight;

    var rect = camera.rect;

    rect.width = scaleWidth;
    rect.height = 1f;
    rect.x = (1f - scaleWidth) / 2f;
    rect.y = 0;

    camera.rect = rect;
  }

  private void SetCameraLetterBoxRect(float scaleHeight)
  {
    var camera = GetComponent<Camera>();
    var rect = camera.rect;

    rect.width = 1f;
    rect.height = scaleHeight;
    rect.x = 0;
    rect.y = (1f - scaleHeight) / 2f;

    camera.rect = rect;
  }

  private bool NeedsLetterBox(float scaleHeight)
  {
    return scaleHeight < 1;
  }

  public float CalculateTargetZAxisPosition()
  {
    return Target.position.z - ZAxisOffset;
  }

  public Vector3 CalculateTargetPosition(CameraMovementSettings cameraMovementSettings)
  {
    var verticalCalculator = CamerPositionCalculatorFactory.CreateVertical(this, cameraMovementSettings);
    var horizontalCalculator = CamerPositionCalculatorFactory.CreateHorizontal(this, cameraMovementSettings);

    return new Vector3(
      horizontalCalculator.CalculateTargetPosition(),
      verticalCalculator.CalculateTargetPosition(),
      CalculateTargetZAxisPosition());
  }

  public bool HasScrollActions()
  {
    return _scrollActionManager.HasActions();
  }

  private bool IsCameraControlledByScrollAction()
  {
    var pos = transform.position;

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
