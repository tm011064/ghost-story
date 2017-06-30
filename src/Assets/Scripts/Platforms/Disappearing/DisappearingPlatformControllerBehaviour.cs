using System.Linq;
using UnityEngine;

public partial class DisappearingPlatformControllerBehaviour : MonoBehaviour
{
  private TimerPlatform[] _platforms;

  private void Awake()
  {
    _platforms = GetComponentsInChildren<TimerPlatform>()
      .OrderBy(p => p.Index)
      .ToArray();

    DisableAllPlatforms();
  }

  public void Show(int index)
  {
    _platforms[index].gameObject.SetActive(true);
  }

  public void Hide(int index)
  {
    _platforms[index].gameObject.SetActive(false);
  }

  public void DisableAllPlatforms()
  {
    foreach (var platform in _platforms)
    {
      platform.gameObject.SetActive(false);
    }
  }
}
