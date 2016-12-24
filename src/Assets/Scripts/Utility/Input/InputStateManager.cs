using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputStateManager
{
  private Dictionary<string, ButtonsState> _buttonStates;

  private Dictionary<string, AxisState> _axisStates;

  public InputStateManager()
  {
    _buttonStates = new Dictionary<string, ButtonsState>();
    _axisStates = new Dictionary<string, AxisState>();
  }

  public ButtonsState GetButtonState(string buttonName)
  {
    try
    {
      return _buttonStates[buttonName];
    }
    catch (KeyNotFoundException)
    {
      Debug.Log("Button " + buttonName + " not found");

      throw;
    }
  }

  public void SetButtonHandled(string buttonName)
  {
    _buttonStates[buttonName].IsHandled = true;
  }

  public bool IsButtonHandled(string buttonName)
  {
    return _buttonStates[buttonName].IsHandled;
  }

  public bool IsButtonDown(string buttonName)
  {
    return (_buttonStates[buttonName].ButtonPressState & ButtonPressState.IsDown) != 0;
  }

  public bool IsButtonPressed(string buttonName)
  {
    return (_buttonStates[buttonName].ButtonPressState & ButtonPressState.IsPressed) != 0;
  }

  public void SetVerticalAxisHandled()
  {
    _axisStates["Vertical"].IsHandled = true;
  }

  public bool IsVerticalAxisHandled()
  {
    return _axisStates["Vertical"].IsHandled;
  }

  public bool IsUpAxisButtonDown(InputSettings inputSettings)
  {
    return _axisStates["Vertical"].Value < inputSettings.AxisSensitivityThreshold
      && _axisStates["Vertical"].HasChangedDirection(inputSettings);
  }

  public bool IsAxisUpPressed(InputSettings inputSettings)
  {
    return _axisStates["Vertical"].Value > inputSettings.AxisSensitivityThreshold;
  }

  public bool IsAxisDownPressed(InputSettings inputSettings)
  {
    return _axisStates["Vertical"].Value > inputSettings.AxisSensitivityThreshold;
  }

  /// <summary>
  /// You can pass in any button name as well as the following axis directions: Down, Up, Left, Right
  /// </summary>
  public bool AreButtonsPressed(InputSettings inputSettings, params string[] buttonAndAxisNames)
  {
    for (var i = 0; i < buttonAndAxisNames.Length; i++)
    {
      switch (buttonAndAxisNames[i])
      {
        case "Down":
          if (_axisStates["Vertical"].Value > -inputSettings.AxisSensitivityThreshold)
          {
            return false;
          }
          break;

        case "Up":
          if (_axisStates["Vertical"].Value < inputSettings.AxisSensitivityThreshold)
          {
            return false;
          }
          break;

        case "Left":
          if (_axisStates["Horizontal"].Value > -inputSettings.AxisSensitivityThreshold)
          {
            return false;
          }
          break;

        case "Right":
          if (_axisStates["Horizontal"].Value < inputSettings.AxisSensitivityThreshold)
          {
            return false;
          }
          break;

        default:
          if ((_buttonStates[buttonAndAxisNames[i]].ButtonPressState & ButtonPressState.IsPressed) == 0)
          {
            return false;
          }
          break;
      }
    }

    return true;
  }

  public AxisState GetVerticalAxisState()
  {
    return _axisStates["Vertical"];
  }

  public AxisState GetHorizontalAxisState()
  {
    return _axisStates["Horizontal"];
  }

  public bool DoesButtonNameExist(string buttonName)
  {
    return _buttonStates.ContainsKey(buttonName);
  }

  public void Update()
  {
    foreach (var value in _buttonStates.Values)
    {
      value.Update();
    }

    foreach (var value in _axisStates.Values)
    {
      value.Update();
    }
  }

  public void InitializeButtons(params string[] buttonNames)
  {
    _buttonStates = buttonNames.ToDictionary(
      name => name,
      name => new ButtonsState(name));
  }

  public void InitializeAxes(params string[] axisNames)
  {
    _axisStates = axisNames.ToDictionary(
      name => name,
      name => new AxisState(name));
  }
}
