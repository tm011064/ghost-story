using System;
using System.Collections.Generic;
using System.Linq;

public class CameraMovementSettingsManager
{
  private readonly List<CameraMovementSettings> _cameraMovementSettings = new List<CameraMovementSettings>();

  public event Action<CameraSettingsChangedArguments> SettingsChanged;

  public CameraMovementSettings ActiveSettings;

  private void ChangeSettings(CameraMovementSettings cameraMovementSettings)
  {
    Logger.UnityDebugLog("CameraMovementSettingsManager -> ChangeSettings");

    ActiveSettings = cameraMovementSettings;

    var actionHandler = SettingsChanged;

    if (actionHandler != null)
    {
      var args = new CameraSettingsChangedArguments { SettingsWereRefreshed = _cameraMovementSettings.Count == 1 };
      actionHandler(args);
    }
  }

  public void AddSettings(CameraMovementSettings cameraMovementSettings)
  {
    Logger.UnityDebugLog("CameraMovementSettingsManager -> AddSettings");

    if (cameraMovementSettings.Equals(ActiveSettings))
    {
      Logger.UnityDebugLog("CameraMovementSettingsManager -> New settings equal active settings");

      return;
    }

    _cameraMovementSettings.Add(cameraMovementSettings);

    ChangeSettings(cameraMovementSettings);
  }

  public void ClearSettings()
  {
    _cameraMovementSettings.Clear();
  }

  public void RemoveSettings(CameraMovementSettings cameraMovementSettings)
  {
    Logger.UnityDebugLog("CameraMovementSettingsManager -> RemoveSettings");

    if (!ActiveSettings.Equals(cameraMovementSettings))
    {
      Logger.UnityDebugLog("CameraMovementSettingsManager -> Active settings do not equal removed settings");

      _cameraMovementSettings.Clear();
      _cameraMovementSettings.Add(ActiveSettings);

      return;
    }

    if (_cameraMovementSettings.Count() == 1)
    {
      Logger.UnityDebugLog("CameraMovementSettingsManager -> Only one setting remains");

      return;
    }

    Logger.UnityDebugLog("CameraMovementSettingsManager -> Removing active settings");

    _cameraMovementSettings.Remove(ActiveSettings);

    ChangeSettings(_cameraMovementSettings.Last());
  }
}