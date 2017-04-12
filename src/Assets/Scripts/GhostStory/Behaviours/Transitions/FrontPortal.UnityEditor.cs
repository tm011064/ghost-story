﻿#if UNITY_EDITOR


namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class FrontPortal : IInstantiable<PrefabInstantiationArguments>
  {
    public void Instantiate(PrefabInstantiationArguments arguments)
    {
      transform.position = arguments.TiledRectBounds.center;

      TransitionToPortalName = arguments.Properties["Transition To Portal"];
      TransitionToScene = arguments.Properties["Transition To Scene"];
      PortalName = arguments.TiledObjectName;

      CreateCameraModifier(arguments);
    }

    private void CreateCameraModifier(PrefabInstantiationArguments arguments)
    {
      return; // TODO (Roman): figure that one out
      //var wrappingCameraBounds = arguments.WrappingCameraBounds.SingleOrDefault();
      //if (wrappingCameraBounds == null)
      //{
      //  return;
      //}

      //var assetPath = arguments.PrefabsAssetPathsByShortName["Camera Modifier"];
      //var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
      //var cameraModifier = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity) as GameObject;
      //cameraModifier.transform.parent = transform;

      //var cameraModifierArguments = new CameraModifierInstantiationArguments
      //{
      //  BoundsPropertyInfos = new CameraModifierInstantiationArguments.BoundsPropertyInfo[]
      //  {
      //    new CameraModifierInstantiationArguments.BoundsPropertyInfo
      //    {
      //      Bounds = arguments.TiledRectBounds,
      //      Properties = arguments.Properties
      //    }
      //  },
      //  Bounds = wrappingCameraBounds
      //};

      //var instantiable = cameraModifier.GetComponentOrThrow<IInstantiable<CameraModifierInstantiationArguments>>();
      //instantiable.Instantiate(cameraModifierArguments);
    }
  }
}

#endif