#if UNITY_EDITOR

using UnityEngine;

public partial class CameraModifier : IInstantiable<CameraModifierInstantiationArguments>
{
  public ImportCameraSettings ImportCameraSettings;

  public void Instantiate(CameraModifierInstantiationArguments arguments)
  {
    var cameraController = Camera.main.GetComponentOrThrow<CameraController>();

    VerticalLockSettings = new VerticalLockSettings
    {
      Enabled = true,
      EnableDefaultVerticalLockPosition = false,
      DefaultVerticalLockPosition = 0f,
      EnableTopVerticalLock = true,
      EnableBottomVerticalLock = true,
      TopVerticalLockPosition = arguments.Bounds.max.y,
      BottomVerticalLockPosition = arguments.Bounds.min.y
    };
    HorizontalLockSettings = new HorizontalLockSettings
    {
      Enabled = true,
      EnableLeftHorizontalLock = true,
      EnableRightHorizontalLock = true,
      LeftHorizontalLockPosition = arguments.Bounds.min.x,
      RightHorizontalLockPosition = arguments.Bounds.max.x
    };

    foreach (var args in arguments.BoundsPropertyInfos)
    {
      var boxColliderGameObject = new GameObject("Box Collider With Enter Trigger");

      boxColliderGameObject.transform.position = args.Bounds.center;
      boxColliderGameObject.layer = gameObject.layer;
      boxColliderGameObject.transform.parent = gameObject.transform;

      var boxCollider = boxColliderGameObject.AddComponent<BoxCollider2D>();

      boxCollider.isTrigger = true;
      boxCollider.size = args.Bounds.size;

      var boxColliderTriggerEnterBehaviour = boxColliderGameObject.AddComponent<BoxColliderTriggerEnterBehaviour>();

      if (args.Properties.GetBoolSafe("Enter On Ladder", false))
      {
        boxColliderTriggerEnterBehaviour.PlayerStatesNeededToEnter = new PlayerState[] { PlayerState.ClimbingLadder };
      }
    }
  }

  void OnDrawGizmos()
  {
    foreach (var collider in GetComponents<EdgeCollider2D>())
    {
      Gizmos.color = GizmoColor;
      Gizmos.DrawLine(collider.points[0], collider.points[1]);

      break;
    }
  }
}

#endif
