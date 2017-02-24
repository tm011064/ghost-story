#if UNITY_EDITOR

using UnityEngine;

public partial class VerticalFollowPlayerCameraPositionCalculator
{
  public void DrawGizmos()
  {
    var xPosFrom = _cameraController.transform.position.x - _cameraController.TargetScreenSize.x;
    var xPosTo = _cameraController.transform.position.x + _cameraController.TargetScreenSize.x;

    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, _cameraPosition, Color.red);
  }
}

#endif