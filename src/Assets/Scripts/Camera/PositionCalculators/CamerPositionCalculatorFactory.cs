using System;

public static class CamerPositionCalculatorFactory
{
  public static ICameraPositionCalculator CreateVertical(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings)
  {
    switch (cameraMovementSettings.CameraSettings.VerticalCameraFollowMode)
    {
      case VerticalCameraFollowMode.FollowWhenGrounded:
        return new VerticalGroundSnappingCalculator(
          cameraMovementSettings,
          cameraController,
          GameManager.Instance.Player);

      case VerticalCameraFollowMode.FollowAlways:
        return new VerticalFollowPlayerCameraPositionCalculator(
          cameraMovementSettings,
          cameraController,
          GameManager.Instance.Player);
    }

    throw new NotSupportedException();
  }

  public static ICameraPositionCalculator CreateHorizontal(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings)
  {
    switch (cameraMovementSettings.CameraSettings.HorizontalCameraFollowMode)
    {
      case HorizontalCameraFollowMode.FollowAlways:
        return new HorizontalFollowPlayerCameraPositionCalculator(
          cameraMovementSettings,
          cameraController,
          GameManager.Instance.Player);
    }

    throw new NotSupportedException();
  }
}
