using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class Door : MonoBehaviour
  {
    public DoorKey DoorKey = DoorKey.Green;
    
    public Vector2 CameraModifierPadding = new Vector2(6, 6);
  }
}
