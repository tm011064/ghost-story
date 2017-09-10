using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public partial class ConnectedTimerPlatformControllerBehaviour : MonoBehaviour
{
  public float Interval = 1;

  public int MaxNumberOfVisiblePlatforms = 3;

  public bool RestartWhenCompleted = false;

  private TimerPlatform[] _platforms;

  private string _showNextTimerName = GhostStoryGameContext.CreateCallbackName(
    typeof(ConnectedTimerPlatformControllerBehaviour), "ShowNext");

  private string _startTimerName = GhostStoryGameContext.CreateCallbackName(
    typeof(ConnectedTimerPlatformControllerBehaviour), "StartTimer");

  public virtual void Awake()
  {
    _platforms = GetComponentsInChildren<TimerPlatform>()
      .OrderBy(p => p.Index)
      .ToArray();

    Assert.IsTrue(MaxNumberOfVisiblePlatforms > 0);
    Assert.IsTrue(_platforms.Length >= MaxNumberOfVisiblePlatforms);
  }

  public virtual void Start()
  {
    StartTimer(); // TODO (Roman): let this be controlled by switch
  }

  public void StartTimer()
  {
    DisableAllPlatforms();

    _platforms[0].gameObject.SetActive(true);

    RegisterShow(1);
  }

  private void RegisterShow(int index)
  {
    GhostStoryGameContext.Instance.RegisterCallback(
      Interval,
      () => Show(index),
      this.GetGameObjectUniverse(),
      _showNextTimerName);
  }

  private void Show(int index)
  {
    if (index >= MaxNumberOfVisiblePlatforms)
    {
      _platforms[index - MaxNumberOfVisiblePlatforms].gameObject.SetActive(false);
    }

    if (index == _platforms.Length - 1 + MaxNumberOfVisiblePlatforms)
    {
      //if (RestartWhenCompleted) // TODO (Roman): switch
      {
        GhostStoryGameContext.Instance.RegisterCallback(
          Interval,
          () => StartTimer(),
          this.GetGameObjectUniverse(),
          _startTimerName);
      }

      return;
    }

    if (index < _platforms.Length)
    {
      _platforms[index].gameObject.SetActive(true);
    }

    RegisterShow(index + 1);
  }

  private void DisableAllPlatforms()
  {
    foreach (var platform in _platforms)
    {
      platform.gameObject.SetActive(false);
    }
  }
}
