#if UNITY_EDITOR

using UnityEngine;

public partial class HorizontalWindowCameraPositionCalculator
{
  public void DrawGizmos()
  {
    var yPosFrom = _cameraController.transform.position.y - _cameraController.TargetScreenSize.y;
    var yPosTo = _cameraController.transform.position.y + _cameraController.TargetScreenSize.y;

    GizmoUtility.DrawVerticalLine(yPosFrom, yPosTo, _cameraPosition, Color.red);
    GizmoUtility.DrawVerticalLine(yPosFrom, yPosTo, _player.transform.position.x + _windowPosition, Color.green);
    GizmoUtility.DrawVerticalLine(yPosFrom, yPosTo, _cameraController.transform.position.x + _windowPosition, Color.cyan);
  }
}

#endif