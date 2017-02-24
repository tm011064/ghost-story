#if UNITY_EDITOR

public partial class CameraController
{
  void OnDrawGizmos()
  {
    if (_verticalCameraPositionCalculator != null)
    {
      _verticalCameraPositionCalculator.DrawGizmos();
    }

    if (_horizontalCameraPositionCalculator != null)
    {
      _horizontalCameraPositionCalculator.DrawGizmos();
    }
  }
}

#endif