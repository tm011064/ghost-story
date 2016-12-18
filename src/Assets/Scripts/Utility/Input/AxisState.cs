using UnityEngine;

public class AxisState
{
  public float Value;

  public float LastValue;

  public bool IsHandled;

  private string _axisName;

  public AxisState(float value)
  {
    this.Value = value;
  }

  public AxisState(string axisName)
  {
    _axisName = axisName;
  }

  public void Update()
  {
    IsHandled = false;

    LastValue = Value;

    Value = Input.GetAxis(_axisName);
  }

  public bool HasChangedDirection(InputSettings inputSettings)
  {
    return
      (
        Mathf.Abs(LastValue) < inputSettings.AxisSensitivityThreshold
        && Mathf.Abs(Value) >= inputSettings.AxisSensitivityThreshold
      )
      ||
      (
        Mathf.Abs(LastValue) >= inputSettings.AxisSensitivityThreshold
        && Mathf.Abs(Value) < inputSettings.AxisSensitivityThreshold
      );
  }

  public AxisState Clone()
  {
    return (AxisState)MemberwiseClone();
  }
}
