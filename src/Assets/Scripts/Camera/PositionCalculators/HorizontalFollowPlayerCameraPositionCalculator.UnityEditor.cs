#if UNITY_EDITOR

using UnityEngine;

public partial class HorizontalFollowPlayerCameraPositionCalculator
{
  public void DrawGizmos()
  {
    var yPosFrom = _cameraController.transform.position.y - _cameraController.TargetScreenSize.y;
    var yPosTo = _cameraController.transform.position.y + _cameraController.TargetScreenSize.y;

    GizmoUtility.DrawVerticalLine(yPosFrom, yPosTo, _cameraPosition, Color.red);
  }
}

#endif