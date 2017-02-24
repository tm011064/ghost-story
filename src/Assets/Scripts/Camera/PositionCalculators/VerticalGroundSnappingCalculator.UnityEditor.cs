#if UNITY_EDITOR

using UnityEngine;

public partial class VerticalGroundSnappingCalculator
{
  public void DrawGizmos()
  {
    var xPosFrom = _cameraController.transform.position.x - _cameraController.TargetScreenSize.x;
    var xPosTo = _cameraController.transform.position.x + _cameraController.TargetScreenSize.x;

    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, _cameraController.transform.position.y, Color.red);
    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, _player.transform.position.y, Color.cyan);
    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, CalculateInnerWindowTopBoundsPosition(), Color.green);
    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, CalculateInnerWindowBottomBoundsPosition(), Color.green);
    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, CalculateOuterWindowTopBoundsPosition(), Color.yellow);
    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, CalculateOuterWindowBottomBoundsPosition(), Color.yellow);
    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, _cameraController.transform.position.y - CalculateUpwardMovementSnapPosition(), Color.gray);
    GizmoUtility.DrawHorizontalLine(xPosFrom, xPosTo, _cameraController.transform.position.y - CalculateDownwardMovementSnapPosition(), Color.white);
  }
}

#endif