#if UNITY_EDITOR

public partial class FullScreenScroller : IInstantiable<FullScreenScrollerInstantiationArguments>
{
  public void Instantiate(FullScreenScrollerInstantiationArguments arguments)
  {
    DestroySpawnedEnemiesOnEnter = arguments.DestroySpawnedEnemiesOnEnter;

    SetPosition(arguments.CameraBounds);

    var boxColliderGameObject = CreateBoxColliderGameObject(arguments.TriggerBounds, "Scroll Trigger");

    boxColliderGameObject.AddComponent<BoxColliderTriggerEnterBehaviour>();
  }
}

#endif