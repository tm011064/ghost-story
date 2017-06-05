using UnityEngine;

public abstract class CameraMovementSettingsBehaviour : MonoBehaviour
{
  public HorizontalCamereaWindowSettings HorizontalCamereaWindowSettings;

  public VerticalSnapWindowSettings VerticalSnapWindowSettings;

  public ZoomSettings ZoomSettings;

  public SmoothDampMoveSettings SmoothDampMoveSettings;

  public CameraSettings CameraSettings;

  protected CameraMovementSettings CameraMovementSettings;

  protected abstract VerticalLockSettings CreateBaseVerticalLockSettings();

  protected abstract HorizontalLockSettings CreateBaseHorizontalLockSettings();

  protected void OverrideSettings(
    SmoothDampMoveSettings smoothDampMoveSettings,
    CameraSettings cameraSettings,
    HorizontalCamereaWindowSettings horizontalCamereaWindowSettings,
    VerticalSnapWindowSettings verticalSnapWindowSettings)
  {
    CameraSettings = cameraSettings;
    SmoothDampMoveSettings = smoothDampMoveSettings;
    VerticalSnapWindowSettings = verticalSnapWindowSettings;
    HorizontalCamereaWindowSettings = horizontalCamereaWindowSettings;

    CameraMovementSettings = CreateCameraMovementSettings();
  }

  protected CameraMovementSettings CreateCameraMovementSettings()
  {
    var cameraController = Camera.main.GetComponent<CameraController>();

    var horizontalLockSettings = CreateHorizontalLockSettings(cameraController);

    var verticalLockSettings = CreateVerticalLockSettings(cameraController);

    return new CameraMovementSettings(
      HorizontalCamereaWindowSettings,
      VerticalSnapWindowSettings,
      verticalLockSettings,
      horizontalLockSettings,
      ZoomSettings,
      SmoothDampMoveSettings,
      CameraSettings);
  }

  private VerticalLockSettings CreateVerticalLockSettings(CameraController cameraController)
  {
    var verticalLockSettings = CreateBaseVerticalLockSettings();

    verticalLockSettings.TopBoundary =
      verticalLockSettings.TopVerticalLockPosition
      - cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    verticalLockSettings.BottomBoundary =
      verticalLockSettings.BottomVerticalLockPosition
      + cameraController.TargetScreenSize.y * .5f / ZoomSettings.ZoomPercentage;

    return verticalLockSettings;
  }

  private HorizontalLockSettings CreateHorizontalLockSettings(CameraController cameraController)
  {
    var horizontalLockSettings = CreateBaseHorizontalLockSettings();

    horizontalLockSettings.LeftBoundary =
      horizontalLockSettings.LeftHorizontalLockPosition
      + cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    horizontalLockSettings.RightBoundary =
      horizontalLockSettings.RightHorizontalLockPosition
      - cameraController.TargetScreenSize.x * .5f / ZoomSettings.ZoomPercentage;

    return horizontalLockSettings;
  }
}
