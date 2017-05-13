#if UNITY_EDITOR

public partial class FullScreenScroller :
  IInstantiable<FullScreenScrollerInstantiationArguments>
{
  public void Instantiate(FullScreenScrollerInstantiationArguments arguments)
  {
    SetPosition(arguments.CameraBounds);

    var boxColliderGameObject = CreateBoxColliderGameObject(arguments.TriggerBounds, "Scroll Trigger");

    var triggerEnterBehaviour = boxColliderGameObject.AddComponent<BoxColliderTriggerEnterBehaviour>();
  }
}

#endif