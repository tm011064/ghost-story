using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class HouseDoor : MonoBehaviour
  {
    public DoorKey DoorKey = DoorKey.GreenHouseDoorKey;

    public HorizontalDirection WallLocation = HorizontalDirection.Left;

    public float PlayerSpawnDistance = 32;

    public string TransitionToScene;

    public string TransitionToSceneObject;

    public Vector2 CameraModifierPadding = new Vector2(6, 6);

    public void SpawnPlayer(PlayerController playerController, Vector3? cameraPosition)
    {

    }
  }
}
