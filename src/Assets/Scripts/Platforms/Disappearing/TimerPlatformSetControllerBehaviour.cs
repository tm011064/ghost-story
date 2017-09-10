using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public partial class TimerPlatformSetControllerBehaviour : MonoBehaviour
{
  public float ActivationDelay = 1;

  public float InvisibleInterval = 1;

  public float VisibleInterval = 1;

  private TimerPlatform[] _platforms;

  private string _showNextTimerName = GhostStoryGameContext.CreateCallbackName(
    typeof(TimerPlatformSetControllerBehaviour), "ShowNext");

  private string _startTimerName = GhostStoryGameContext.CreateCallbackName(
    typeof(TimerPlatformSetControllerBehaviour), "StartTimer");

  public virtual void Awake()
  {
    _platforms = GetComponentsInChildren<TimerPlatform>()
      .OrderBy(p => p.Index)
      .ToArray();

    Assert.IsTrue(_platforms.Length > 0);
  }

  public virtual void Start()
  {
    StartTimer();
  }

  public void StartTimer()
  {
    DisableAllPlatforms();

    var startDelay = .0f;
    _platforms.ForEach(platform =>
    {
      GhostStoryGameContext.Instance.RegisterCallback(
        startDelay,
        () => Show(platform),
        this.GetGameObjectUniverse(),
        _showNextTimerName);

      startDelay += ActivationDelay;
    });
  }

  private void Show(TimerPlatform platform)
  {
    platform.gameObject.SetActive(true);

    GhostStoryGameContext.Instance.RegisterCallback(
      VisibleInterval,
      () => Hide(platform),
      this.GetGameObjectUniverse(),
      _startTimerName);
  }

  private void Hide(TimerPlatform platform)
  {
    platform.gameObject.SetActive(false);

    GhostStoryGameContext.Instance.RegisterCallback(
      InvisibleInterval,
      () => Show(platform),
      this.GetGameObjectUniverse(),
      _startTimerName);
  }

  private void DisableAllPlatforms()
  {
    foreach (var platform in _platforms)
    {
      platform.gameObject.SetActive(false);
    }
  }
}
