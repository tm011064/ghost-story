using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerPlatformBuilder
{
  private readonly Transform _transform;

  public TimerPlatformBuilder(Transform parent)
  {
    _transform = parent;
  }

  public IEnumerable<TimerPlatform> Build(IEnumerable<PlatformArguments> platforms)
  {
    return platforms.Select(p => Create(p));
  }

  private TimerPlatform Create(PlatformArguments platformArgs)
  {
    var platform = CreatePlatform(platformArgs);

    AttachSprite(platform, platformArgs);

    var timerPlatform = AddTimerPlatform(platform, platformArgs);

    platformArgs.ColliderObjects.ForEach(obj => obj.transform.parent = platform.transform);

    return timerPlatform;
  }

  private GameObject CreatePlatform(PlatformArguments platformArgs)
  {
    var platform = new GameObject()
    {
      name = "Platform " + platformArgs.Index
    };

    platform.transform.parent = _transform;
    platform.layer = LayerMask.NameToLayer("Platforms");

    return platform;
  }

  private void AttachSprite(GameObject platform, PlatformArguments platformArgs)
  {
    var meshRenderer = platformArgs.Transform.GetComponentInChildren<MeshRenderer>();

    meshRenderer.transform.parent = platform.transform;
    meshRenderer.gameObject.name = "Sprite";
    meshRenderer.transform.position = meshRenderer.transform.position;

    Object.DestroyImmediate(platformArgs.Transform.gameObject);
  }

  private TimerPlatform AddTimerPlatform(GameObject platform, PlatformArguments platformArgs)
  {
    var timerPlatform = platform.AddComponent<TimerPlatform>();

    timerPlatform.Index = platformArgs.Index;

    return timerPlatform;
  }
}
