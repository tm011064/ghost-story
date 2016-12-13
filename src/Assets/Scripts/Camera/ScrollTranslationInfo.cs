using System.Collections.Generic;

public class ScrollTranslationInfo
{
  public ScrollTranslationInfo(
    TranslateTransformAction[] translateTransformActions,
    float playerMovementDelayDuration,
    AxisType playerMovementAxis)
  {
    TranslateTransformActions = translateTransformActions;
    PlayerMovementDelayDuration = playerMovementDelayDuration;
    PlayerMovementAxis = playerMovementAxis;
  }

  public IEnumerable<TranslateTransformAction> TranslateTransformActions { get; private set; }

  public float PlayerMovementDelayDuration { get; private set; }

  public AxisType PlayerMovementAxis { get; private set; }
}
