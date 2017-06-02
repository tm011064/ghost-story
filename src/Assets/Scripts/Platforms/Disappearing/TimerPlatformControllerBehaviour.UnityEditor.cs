#if UNITY_EDITOR

using UnityEngine;

public partial class TimerPlatformControllerBehaviour : IInstantiable<TimerPlatformInstantiationArguments>
{
  public void Instantiate(TimerPlatformInstantiationArguments arguments)
  {
    foreach (var platformArgs in arguments.Platforms)
    {
      var platform = CreatePlatform(platformArgs);

      AttachSprite(platform, platformArgs);

      AddTimerPlatform(platform, platformArgs);

      AddBoxCollider(platform, platformArgs);
    }
  }

  private GameObject CreatePlatform(TimerPlatformInstantiationArguments.Platform platformArgs)
  {
    var platform = new GameObject();

    platform.transform.position = platformArgs.Bounds.center.AddY(platformArgs.Bounds.size.y / 2);
    platform.name = "Platform " + platformArgs.Index;
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
  }

  private void AddBoxCollider(GameObject platform, TimerPlatformInstantiationArguments.Platform platformArgs)
  {
    var boxCollider = platform.AddComponent<BoxCollider2D>();

    boxCollider.isTrigger = false;
    boxCollider.size = platformArgs.Bounds.size;
  }

  private void AddTimerPlatform(GameObject platform, TimerPlatformInstantiationArguments.Platform platformArgs)
  {
    var timerPlatform = platform.AddComponent<TimerPlatform>();

    timerPlatform.Index = platformArgs.Index;
  }
}

#endif