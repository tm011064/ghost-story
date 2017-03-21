using Assets.Scripts.GhostStory.Behaviours.Transitions;
using UnityEngine;

public class HouseDoorFadeOutBehaviour : MonoBehaviour
{
  public float PlayerTranslationDistance;

  private HouseDoor HouseDoor;

  private DoorTriggerEnterBehaviour _doorTriggerEnterBehaviour;

  void Awake()
  {
    _doorTriggerEnterBehaviour = GetComponentInChildren<DoorTriggerEnterBehaviour>();
    _doorTriggerEnterBehaviour.Open += TriggerScroll;

    HouseDoor = GetComponentInParent<HouseDoor>();
    PlayerTranslationDistance = GhostStoryGameContext.Instance.GameSettings.FullScreenScrollSettings.PlayerTranslationDistance;
  }

  void OnDestroy()
  {
    _doorTriggerEnterBehaviour.Open -= TriggerScroll;
  }

  private void MovePlayerIntoDoor()
  {
    var directionMultiplier = GameManager.Instance.Player.IsFacingRight() ? 1 : -1;

    GameManager.Instance.Player.PushControlHandlers(
      new FreezePlayerControlHandler(
        GameManager.Instance.Player,
        -1,
        Animator.StringToHash("Idle"),
        new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }),
      new TranslateFrozenPlayerControlHandler(
        GameManager.Instance.Player,
        .6f,
        Animator.StringToHash("Run Start"),
        new Vector3(PlayerTranslationDistance / 2 * directionMultiplier, 0, 0),
        EasingType.Linear),
      new FreezePlayerControlHandler(
        GameManager.Instance.Player,
        .2f,
        Animator.StringToHash("Idle"),
        new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }));
  }

  public void TriggerScroll()
  {
    if (HouseDoor.IsTransitionDoorActive())
    {
      return;
    }

    MovePlayerIntoDoor();

    GhostStoryGameContext.Instance.RegisterCallback(
      .8f,
      () => GameManager.Instance.SceneManager.FadeOut(() => OnFadeOutCompleted()),
      "FadeOut");

    ShowTransitionDoor();
  }

  void ShowTransitionDoor()
  {
    HouseDoor.Door.SetActive(false);
    HouseDoor.TransitionDoor.SetActive(true);
  }

  void LoadScene()
  {
    var doorPositionRelativeToCameraPosition = transform.position - Camera.main.transform.position;

    GameManager.Instance.SceneManager.LoadScene(
      HouseDoor.TransitionToScene,
      HouseDoor.TransitionToPortalName,
      doorPositionRelativeToCameraPosition);
  }

  void OnFadeOutCompleted()
  {
    GhostStoryGameContext.Instance.RegisterCallback(.4f, LoadScene, "LoadScene");
  }
}
