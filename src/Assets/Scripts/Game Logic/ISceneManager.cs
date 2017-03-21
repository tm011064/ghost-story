using System;
using UnityEngine;

public interface ISceneManager
{
  void FadeIn(Action onFadeCompleted = null);

  void FadeOut(Action onFadeCompleted = null);

  void LoadScene(string sceneName, string startPointPrefabName, Vector3 fromPortalPosition);

  void ShowBlackScreen();

  void FocusCameraOnPlayer();

  void OnSceneLoad();

  bool IsFading();

  bool IsLoading();
}
