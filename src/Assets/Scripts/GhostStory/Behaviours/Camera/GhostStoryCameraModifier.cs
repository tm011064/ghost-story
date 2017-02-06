using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GhostStoryCameraModifier : CameraModifier
{
  protected override void OnAwake()
  {
    SmoothDampMoveSettings = GhostStoryGameContext.Instance.GameSettings.SmoothDampMoveSettings;
    CameraSettings = GhostStoryGameContext.Instance.GameSettings.CameraSettings;
  }
}
