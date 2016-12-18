using UnityEngine;

public class ButtonsState
{
  public ButtonPressState ButtonPressState;

  private string _buttonName;

  private float _pressStarted;

  public bool IsHandled;

  public void Update()
  {
    IsHandled = false;

    var pressState = ButtonPressState.Idle;

    if (Input.GetButton(_buttonName))
    {
      pressState |= ButtonPressState.IsPressed;
    }

    if (((pressState & ButtonPressState.IsPressed) != 0)
      && ((ButtonPressState & ButtonPressState.IsPressed) == 0))
    {
      _pressStarted = Time.time;

      pressState |= ButtonPressState.IsDown;
    }

    if (((pressState & ButtonPressState.IsPressed) == 0)
      && ((ButtonPressState & ButtonPressState.IsPressed) != 0))
    {
      pressState |= ButtonPressState.IsUp;
    }

    if ((pressState & (ButtonPressState.IsUp | ButtonPressState.IsDown | ButtonPressState.IsPressed)) != 0)
    {
      pressState &= ~ButtonPressState.Idle;
    }

    ButtonPressState = pressState;
  }

  public float GetPressedTime()
  {
    if (((ButtonPressState & ButtonPressState.IsPressed) == 0))
    {
      return 0f;
    }

    return Time.time - _pressStarted;
  }

  public ButtonsState(string buttonName)
  {
    _buttonName = buttonName;
  }
}
