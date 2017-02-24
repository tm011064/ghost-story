using System;
using System.Collections.Generic;
using System.Linq;

public class CameraMovementSettingsManager
{
  private readonly List<CameraMovementSettings> _cameraMovementSettings = new List<CameraMovementSettings>();

  public event Action SettingsChanged;

  public CameraMovementSettings ActiveSettings;

  public int SettingsCount { get { return _cameraMovementSettings.Count(); } }

  private void ChangeSettings(CameraMovementSettings cameraMovementSettings)
  {
    ActiveSettings = cameraMovementSettings;
    Logger.UnityDebugLog("CameraMovementSettingsManager -> ChangeSettings, count: " + _cameraMovementSettings.Count + ", " + ActiveSettings.ToString());

    var handler = SettingsChanged;
    if (handler != null)
    {
      handler();
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
    Logger.UnityDebugLog("CameraMovementSettingsManager -> Clearing all settings");

    ActiveSettings = null;
    _cameraMovementSettings.Clear();
  }

  public void RemoveSettings(CameraMovementSettings cameraMovementSettings)
  {
    if (!ActiveSettings.Equals(cameraMovementSettings))
    {
      Logger.UnityDebugLog("CameraMovementSettingsManager -> Active settings do not equal removed settings");

      _cameraMovementSettings.Clear();
      _cameraMovementSettings.Add(ActiveSettings);

      return;
    }

    if (_cameraMovementSettings.Count() == 1)
    {
      Logger.UnityDebugLog("CameraMovementSettingsManager -> Active setting is last");

      return;
    }

    Logger.UnityDebugLog("CameraMovementSettingsManager -> Removing active settings");

    _cameraMovementSettings.Remove(ActiveSettings);

    ChangeSettings(_cameraMovementSettings.Last());
  }
}