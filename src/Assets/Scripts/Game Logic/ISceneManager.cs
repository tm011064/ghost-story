using System;
public interface ISceneManager
{
  void FadeIn(Action onFadeCompleted = null);

  void FadeOut(Action onFadeCompleted = null);

  void LoadScene(string sceneName, string startPointPrefabName);
}
