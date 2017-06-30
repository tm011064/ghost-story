#if UNITY_EDITOR

using UnityEngine;

public partial class DisappearingPlatformControllerBehaviour : IInstantiable<TimerPlatformInstantiationArguments>
{
  public void Instantiate(TimerPlatformInstantiationArguments arguments)
  {
    foreach (var platformArgs in arguments.Platforms)
    {
      var platform = CreatePlatform(platformArgs);

      AttachSprite(platform, platformArgs);

      AddTimerPlatform(platform, platformArgs);

      foreach (var obj in platformArgs.ColliderObjects)
      {
        obj.transform.parent = platform.transform;
      }
    }
  }

  private GameObject CreatePlatform(TimerPlatformInstantiationArguments.Platform platformArgs)
  {
    var platform = new GameObject()
    {
      name = "Platform " + platformArgs.Index
    };

    platform.transform.parent = transform;
    platform.layer = LayerMask.NameToLayer("Platforms");

    return platform;
  }

  private void AttachSprite(GameObject platform, TimerPlatformInstantiationArguments.Platform platformArgs)
  {
    var meshRenderer = platformArgs.Transform.GetComponentInChildren<MeshRenderer>();

    meshRenderer.transform.parent = platform.transform;
    meshRenderer.gameObject.name = "Sprite";
    meshRenderer.transform.position = meshRenderer.transform.position;

    DestroyImmediate(platformArgs.Transform.gameObject);
  }

  private void AddTimerPlatform(GameObject platform, TimerPlatformInstantiationArguments.Platform platformArgs)
  {
    var timerPlatform = platform.AddComponent<TimerPlatform>();

    timerPlatform.Index = platformArgs.Index;
  }
}

#endif