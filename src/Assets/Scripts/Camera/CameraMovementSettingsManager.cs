﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMovementSettingsManager
{
  private readonly List<CameraMovementSettings> _cameraMovementSettings = new List<CameraMovementSettings>();

  public event Action SettingsChanged;

  public CameraMovementSettings ActiveSettings;

  private void ChangeSettings(CameraMovementSettings cameraMovementSettings)
  {
    ActiveSettings = cameraMovementSettings;

    var actionHandler = SettingsChanged;

    if (actionHandler != null)
    {
      actionHandler();
    }
  }

  public void AddSettings(CameraMovementSettings cameraMovementSettings)
  {
    if (cameraMovementSettings.Equals(ActiveSettings))
    {
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
    if (!ActiveSettings.Equals(cameraMovementSettings))
    {
      _cameraMovementSettings.Clear();
      _cameraMovementSettings.Add(ActiveSettings);

      return;
    }

    if (_cameraMovementSettings.Count() == 1)
    {
      return;
    }

    _cameraMovementSettings.Remove(ActiveSettings);

    ChangeSettings(_cameraMovementSettings.Last());
  }
}