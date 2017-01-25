using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class DoorCameraScroller : CameraScroller // TODO (Roman): inherit from MonoBehaviour
  {
    protected override void OnCameraScrollCompleted()
    {
      GhostStoryGameContext.Instance.RegisterCallback(
        .3f,
        () => GameManager.Instance.SceneManager.FadeIn(OnFadeInCompleted),
        "FadeIn");

      CameraController.ClearCameraModifiers();
      CameraController.OnCameraModifierEnter(CameraMovementSettings);
    }

    public void TriggerScroll(Collider2D collider)
    {
      var directionMultiplier = GameManager.Instance.Player.IsFacingRight()
        ? 1
        : -1;

      GameManager.Instance.Player.PushControlHandlers(
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          .4f,
          Animator.StringToHash("Idle"),
          new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }
          ),
        new TranslateFrozenPlayerControlHandler(
          GameManager.Instance.Player,
          .5f,
          Animator.StringToHash("Run Start"),
          new Vector3(42 * directionMultiplier, 0, 0),
          EasingType.Linear),
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          2.4f,
          Animator.StringToHash("Idle"),
          new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }
          ),
        new TranslateFrozenPlayerControlHandler(
          GameManager.Instance.Player,
          .6f,
          Animator.StringToHash("Run Start"),
          new Vector3(42 * directionMultiplier, 0, 0),
          EasingType.Linear),
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          .2f,
          Animator.StringToHash("Idle"),
          new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }
          ));

      GhostStoryGameContext.Instance.RegisterCallback(.4f, FadeOut, "FadeOut");

      var leftDoor = gameObject.transform.parent.Find("Left Door");
      leftDoor.gameObject.SetActive(false);

      var rightDoor = gameObject.transform.parent.Find("Right Door");
      rightDoor.gameObject.SetActive(false);

      var transitionDoor = GameManager.Instance.Player.IsFacingRight()
        ? gameObject.transform.parent.Find("Left Transition Door")
        : gameObject.transform.parent.Find("Right Transition Door");
      transitionDoor.gameObject.SetActive(true);
    }

    void FadeOut()
    {
      GameManager.Instance.SceneManager.FadeOut(() => OnFadeOutCompleted());
    }

    void SwitchDoors()
    {
      var leftDoor = gameObject.transform.parent.Find("Left Door");
      leftDoor.gameObject.SetActive(true);

      var rightDoor = gameObject.transform.parent.Find("Right Door");
      rightDoor.gameObject.SetActive(true);

      gameObject.transform.parent.Find("Right Transition Door").gameObject.SetActive(false);
      gameObject.transform.parent.Find("Left Transition Door").gameObject.SetActive(false);
    }

    void OnFadeInCompleted()
    {
      GhostStoryGameContext.Instance.RegisterCallback(.25f, SwitchDoors, "SwitchDoors");
    }

    void StartCameraScroll()
    {
      var targetPosition = CameraController.CalculateTargetPosition(CameraMovementSettings);
      var player = GameManager.Instance.Player;
      var cameraPosition = CameraController.Transform.position;

      var contexts = PlayerTranslationActionContextFactory.Create(
        cameraPosition,
        targetPosition,
        FullScreenScrollerTransitionMode,
        -1,
        FullScreenScrollSettings).ToArray();

      ScrollActions = new TranslateTransformActions(contexts.Select(c => c.TranslateTransformAction));

      CameraController.RegisterScrollActions(ScrollActions);
      Status = ScrollStatus.Scrolling;
    }

    void OnFadeOutCompleted()
    {
      GhostStoryGameContext.Instance.RegisterCallback(.4f, StartCameraScroll, "StartCameraScroll");
    }
  }
}
