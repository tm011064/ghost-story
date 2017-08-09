#if UNITY_EDITOR

using UnityEngine;

public partial class CameraScroller
{
  public Color OutlineGizmoColor = Color.yellow;

  public bool ShowGizmoOutline = true;
  
  protected void SetPosition(Bounds bounds)
  {
    transform.position = bounds.center;

    Size = bounds.size;
  }

  public bool Contains(Vector2 point)
  {
    var bounds = new Bounds(transform.position, Size);

    return bounds.Contains(point);
  }

  void OnDrawGizmos()
  {
    if (ShowGizmoOutline)
    {
      GizmoUtility.DrawBoundingBox(transform.position, Size / 2, OutlineGizmoColor);
    }
  }

  protected GameObject CreateBoxColliderGameObject(Bounds bounds, string name = "Box Collider With Enter Trigger")
  {
    var boxColliderGameObject = new GameObject(name);

    boxColliderGameObject.transform.position = bounds.center;
    boxColliderGameObject.layer = gameObject.layer;
    boxColliderGameObject.transform.parent = gameObject.transform;

    var boxCollider = boxColliderGameObject.AddComponent<BoxCollider2D>();

    boxCollider.isTrigger = true;
    boxCollider.size = bounds.size;

    return boxColliderGameObject;
  }
}
#endif
