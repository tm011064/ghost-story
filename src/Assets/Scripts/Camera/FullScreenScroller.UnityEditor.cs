#if UNITY_EDITOR

public partial class FullScreenScroller : IInstantiable<CameraModifierInstantiationArguments>, IInstantiable<InstantiationArguments>
{
  public void Instantiate(InstantiationArguments arguments)
  {
    SetPosition(arguments.Bounds);

    transform.ForEachChildComponent<IInstantiable<InstantiationArguments>>(
      instantiable => instantiable.Instantiate(arguments));
  }

  public void Instantiate(CameraModifierInstantiationArguments arguments)
  {
    SetPosition(arguments.Bounds);

    foreach (var args in arguments.BoundsPropertyInfos)
    {
      var boxColliderGameObject = CreateBoxColliderGameObject(args.Bounds);

      var boxColliderTriggerEnterBehaviour = boxColliderGameObject.AddComponent<BoxColliderTriggerEnterBehaviour>();

      if (args.Properties.GetBoolSafe("Enter On Ladder", false))
      {
        boxColliderTriggerEnterBehaviour.PlayerStatesNeededToEnter = new PlayerState[] { PlayerState.ClimbingLadder };
      }
    }
  }
}

#endif