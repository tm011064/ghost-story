using UnityEngine;

public class CameraPositionCalculator
{
  public Vector3 CalculateTargetPosition(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings,
    out UpdateParameters updateParameters)
  {
    updateParameters = new UpdateParameters();
    updateParameters.IsAboveJumpHeightLocked = cameraController.IsAboveJumpHeightLocked;

    CalculateVerticalCameraPosition(cameraController, cameraMovementSettings, ref updateParameters);

    CalculateHorizontalCameraPosition(cameraController, cameraMovementSettings, ref updateParameters);

    return new Vector3(
      updateParameters.XPos,
      updateParameters.YPos - cameraController.CameraOffset.y,
      cameraController.Target.position.z - cameraController.CameraOffset.z);
  }

  private bool IsCameraOnTrolley(CameraController cameraController, ref UpdateParameters updateParameters)
  {
    if (cameraController.CameraTrolleys == null)
    {
      return false;
    }

    for (var i = 0; i < cameraController.CameraTrolleys.Length; i++)
    {
      if (!cameraController.CameraTrolleys[i].IsPlayerWithinBoundingBox)
      {
        continue;
      }

      float? posY = cameraController.CameraTrolleys[i].GetPositionY(cameraController.Target.position.x);

      if (posY.HasValue)
      {
        updateParameters.YPos = posY.Value;

        return true;
      }

      break;
    }

    return false;
  }

  private void CalculateVerticalCameraPosition(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings,
    ref UpdateParameters updateParameters)
  {
    if (IsCameraOnTrolley(cameraController, ref updateParameters))
    {
      updateParameters.VerticalSmoothDampTime = cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime;

      return;
    }

    switch (cameraMovementSettings.CameraSettings.VerticalCameraFollowMode)
    {
      case VerticalCameraFollowMode.FollowWhenGrounded:
        AdjustFollowWhenGroundedParameters(cameraController, cameraMovementSettings, ref updateParameters);
        break;

      case VerticalCameraFollowMode.FollowAlways:
      default:
        AdjustFollowAlwaysParameters(cameraController, cameraMovementSettings, ref updateParameters);
        break;
    }

    updateParameters.VerticalSmoothDampTime = updateParameters.DoSmoothDamp // override
      ? cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime
      : updateParameters.IsFallingDown
        ? cameraMovementSettings.SmoothDampMoveSettings.VerticalRapidDescentSmoothDampTime
        : updateParameters.IsAboveJumpHeightLocked
          ? cameraMovementSettings.SmoothDampMoveSettings.VerticalAboveRapidAcsentSmoothDampTime
          : cameraMovementSettings.SmoothDampMoveSettings.VerticalSmoothDampTime;
  }

  private void UnlockAboveJumpHeightWhenPlayerMovesDownwards(ref UpdateParameters updateParameters)
  {
    if (updateParameters.IsAboveJumpHeightLocked && GameManager.Instance.Player.CharacterPhysicsManager.Velocity.y < 0f)
    {
      updateParameters.IsAboveJumpHeightLocked = false;
    }
  }

  private bool IsPlayerAboveTopVerticalLock(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings)
  {
    return cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
       && cameraController.Target.position.y > cameraMovementSettings.VerticalLockSettings.TopBoundary;
  }

  private bool CameraCanMoveUp(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings)
  {
    return !cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
      || cameraController.Target.position.y <= cameraMovementSettings.VerticalLockSettings.TopBoundary;
  }

  private bool IsPlayerAboveMaxPossibleJumpHeight(CameraController cameraController)
  {
    return cameraController.Target.position.y >
      cameraController.Transform.position.y
      + cameraController.CameraOffset.y
      + GameManager.Instance.Player.CalculateMaxJumpHeight();
  }

  private void AdjustFollowWhenGroundedParameters(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings,
    ref UpdateParameters updateParameters)
  {
    UnlockAboveJumpHeightWhenPlayerMovesDownwards(ref updateParameters);

    if (updateParameters.IsAboveJumpHeightLocked)
    {
      if (IsPlayerAboveTopVerticalLock(cameraController, cameraMovementSettings))
      {
        updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.TopBoundary + cameraController.CameraOffset.y;
        updateParameters.DoSmoothDamp = true;
        return;
      }

      if (GameManager.Instance.Player.CharacterPhysicsManager.Velocity.y > 0f
        && CameraCanMoveUp(cameraController, cameraMovementSettings)
        && IsPlayerAboveMaxPossibleJumpHeight(cameraController))
      {
        updateParameters.YPos = cameraController.Target.position.y - cameraController.GameManager.Player.JumpSettings.RunJumpHeight;
        updateParameters.IsAboveJumpHeightLocked = true;
        return;
      }
    }

    updateParameters.IsFallingDown = (GameManager.Instance.Player.CharacterPhysicsManager.Velocity.y < 0f
       && (cameraController.Target.position.y < cameraController.Transform.position.y + cameraController.CameraOffset.y));

    if (GameManager.Instance.Player.IsAirborne() && !updateParameters.IsFallingDown)
    {
      if (cameraMovementSettings.VerticalLockSettings.Enabled
        && cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
        && cameraController.Target.position.y + cameraController.CameraOffset.y > cameraMovementSettings.VerticalLockSettings.TopBoundary)
      {
        updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.TopBoundary + cameraController.CameraOffset.y;
        updateParameters.DoSmoothDamp = true;
        return;
      }

      updateParameters.YPos = cameraController.Transform.position.y + cameraController.CameraOffset.y;
      return;
    }

    if (!cameraMovementSettings.VerticalLockSettings.Enabled)
    {
      updateParameters.YPos = cameraController.Target.position.y;
      return;
    }

    if (cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock)
    {
      if (cameraController.Target.position.y > cameraMovementSettings.VerticalLockSettings.TopBoundary + cameraController.CameraOffset.y)
      {
        updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.TopBoundary + cameraController.CameraOffset.y;
        updateParameters.DoSmoothDamp = true;
        return;
      }

      if (cameraController.Target.position.y < cameraMovementSettings.VerticalLockSettings.BottomBoundary)
      {
        updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.BottomBoundary + cameraController.CameraOffset.y;
        updateParameters.DoSmoothDamp = true;
        return;
      }
    }

    if (cameraMovementSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition)
    {
      updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.DefaultVerticalLockPosition;
      return;
    }

    updateParameters.YPos = cameraController.Target.position.y;
  }

  private void AdjustFollowAlwaysParameters(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings,
    ref UpdateParameters updateParameters)
  {
    updateParameters.IsAboveJumpHeightLocked = false; // this is not used at this mode

    if (cameraMovementSettings.VerticalLockSettings.Enabled)
    {
      updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.EnableDefaultVerticalLockPosition
        ? cameraMovementSettings.VerticalLockSettings.DefaultVerticalLockPosition
        : cameraController.Target.position.y;

      if (cameraMovementSettings.VerticalLockSettings.EnableTopVerticalLock
        && cameraController.Target.position.y > cameraMovementSettings.VerticalLockSettings.TopBoundary + cameraController.CameraOffset.y)
      {
        updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.TopBoundary + cameraController.CameraOffset.y;

        // we might have been shot up, so use smooth damp override
        updateParameters.DoSmoothDamp = true;
      }
      else if (cameraMovementSettings.VerticalLockSettings.EnableBottomVerticalLock
        && cameraController.Target.position.y < cameraMovementSettings.VerticalLockSettings.BottomBoundary + cameraController.CameraOffset.y)
      {
        updateParameters.YPos = cameraMovementSettings.VerticalLockSettings.BottomBoundary + cameraController.CameraOffset.y;

        // we might have been falling down, so use smooth damp override
        updateParameters.DoSmoothDamp = true;
      }
    }
    else
    {
      updateParameters.YPos = cameraController.Target.position.y;
    }
  }

  private void CalculateHorizontalCameraPosition(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings,
    ref UpdateParameters updateParameters)
  {
    updateParameters.XPos = cameraController.Target.position.x;

    if (NeedsHorizontalOffsetAdjustment(cameraController, cameraMovementSettings, ref updateParameters))
    {
      AdjustHorizontalOffset(cameraController, cameraMovementSettings, ref updateParameters);
    }
  }

  private bool NeedsHorizontalOffsetAdjustment(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings,
    ref UpdateParameters updateParameters)
  {
    if (cameraMovementSettings.HorizontalLockSettings.Enabled)
    {
      if (cameraMovementSettings.HorizontalLockSettings.EnableRightHorizontalLock
        && cameraController.Target.position.x > cameraMovementSettings.HorizontalLockSettings.RightBoundary - cameraController.CameraOffset.x)
      {
        updateParameters.XPos = cameraMovementSettings.HorizontalLockSettings.RightBoundary;

        return false;
      }
      else if (cameraMovementSettings.HorizontalLockSettings.EnableLeftHorizontalLock
        && cameraController.Target.position.x < cameraMovementSettings.HorizontalLockSettings.LeftBoundary + cameraController.CameraOffset.x)
      {
        updateParameters.XPos = cameraMovementSettings.HorizontalLockSettings.LeftBoundary;

        return false;
      }
    }

    return cameraController.CameraOffset.x != 0f;
  }

  private void AdjustHorizontalOffset(
    CameraController cameraController,
    CameraMovementSettings cameraMovementSettings,
    ref UpdateParameters updateParameters)
  {
    var horizontalTargetDistance = cameraController.Target.transform.position.x - cameraController.LastTargetPosition.x;

    updateParameters.XPos = cameraController.TargetedTransformPositionX;

    if ((horizontalTargetDistance >= -.001f && horizontalTargetDistance <= .001f)
      || cameraController.CameraOffset.x >= 0f)
    {
      return;
    }

    updateParameters.XPos =
      cameraController.TargetedTransformPositionX
      + horizontalTargetDistance * cameraMovementSettings.CameraSettings.HorizontalOffsetDeltaMovementFactor;

    if (horizontalTargetDistance > 0f) // going right
    {
      if (updateParameters.XPos + cameraController.CameraOffset.x > cameraController.Target.position.x)
      {
        updateParameters.XPos = cameraController.Target.position.x - cameraController.CameraOffset.x;
      }

      if (cameraMovementSettings.HorizontalLockSettings.EnableRightHorizontalLock
        && updateParameters.XPos > cameraMovementSettings.HorizontalLockSettings.RightBoundary)
      {
        updateParameters.XPos = cameraMovementSettings.HorizontalLockSettings.RightBoundary;
      }
    }
    else // going left
    {
      if (updateParameters.XPos - cameraController.CameraOffset.x < cameraController.Target.position.x)
      {
        updateParameters.XPos = cameraController.Target.position.x + cameraController.CameraOffset.x;
      }

      if (cameraMovementSettings.HorizontalLockSettings.EnableLeftHorizontalLock
        && updateParameters.XPos < cameraMovementSettings.HorizontalLockSettings.LeftBoundary)
      {
        updateParameters.XPos = cameraMovementSettings.HorizontalLockSettings.LeftBoundary;
      }
    }
  }

  public struct UpdateParameters
  {
    public float YPos;

    public float XPos;

    public bool DoSmoothDamp;

    public bool IsFallingDown;

    public float VerticalSmoothDampTime;

    public bool IsAboveJumpHeightLocked;
  }
}
