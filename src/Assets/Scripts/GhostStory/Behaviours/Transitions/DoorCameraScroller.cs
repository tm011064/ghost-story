﻿using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class DoorCameraScroller : CameraScroller
  {
    private GameObject _leftDoor;

    private GameObject _rightDoor;

    private GameObject _leftTransitionDoor;

    private GameObject _rightTransitionDoor;

    private BaseControlHandler _freezeControlHandler;

    protected override void OnAwake()
    {
      _leftDoor = gameObject.transform.parent.Find("Left Door").gameObject;
      _rightDoor = gameObject.transform.parent.Find("Right Door").gameObject;
      _leftTransitionDoor = gameObject.transform.parent.Find("Left Transition Door").gameObject;
      _rightTransitionDoor = gameObject.transform.parent.Find("Right Transition Door").gameObject;
    }

    private void MovePlayerIntoDoor()
    {
      var directionMultiplier = GameManager.Instance.Player.IsFacingRight() ? 1 : -1;

      _freezeControlHandler = new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          -1,
          Animator.StringToHash("Idle"),
          new PlayerState[] { PlayerState.Invincible, PlayerState.Locked });

      GameManager.Instance.Player.PushControlHandlers(
        _freezeControlHandler,
        new TranslateFrozenPlayerControlHandler(
          GameManager.Instance.Player,
          .6f,
          Animator.StringToHash("Run Start"),
          new Vector3(FullScreenScrollSettings.PlayerTranslationDistance / 2 * directionMultiplier, 0, 0),
          EasingType.Linear),
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          .2f,
          Animator.StringToHash("Idle"),
          new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }));
    }

    private void MovePlayerIntoRoom()
    {
      var directionMultiplier = GameManager.Instance.Player.IsFacingRight() ? 1 : -1;

      GameManager.Instance.Player.PushControlHandlers(
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          .4f,
          Animator.StringToHash("Idle"),
          new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }),
        new TranslateFrozenPlayerControlHandler(
          GameManager.Instance.Player,
          .5f,
          Animator.StringToHash("Run Start"),
          new Vector3(FullScreenScrollSettings.PlayerTranslationDistance / 2 * directionMultiplier, 0, 0),
          EasingType.Linear),
        new FreezePlayerControlHandler(
          GameManager.Instance.Player,
          .2f,
          Animator.StringToHash("Idle"),
          new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }));

      GameManager.Instance.Player.RemoveControlHandler(_freezeControlHandler);
    }

    protected override void OnCameraScrollCompleted()
    {
      MovePlayerIntoRoom();

      GhostStoryGameContext.Instance.RegisterCallback(
        .3f,
        () => GameManager.Instance.SceneManager.FadeIn(OnFadeInCompleted),
        "FadeIn");

      CameraController.ClearCameraModifiers();
      CameraController.OnCameraModifierEnter(CameraMovementSettings);
    }

    public void TriggerScroll(Collider2D collider)
    {
      MovePlayerIntoDoor();

      GhostStoryGameContext.Instance.RegisterCallback(
        .8f,
        () => GameManager.Instance.SceneManager.FadeOut(() => OnFadeOutCompleted()),
        "FadeOut");

      ShowTransitionDoor();
    }

    private void SetWallDoorsActive(bool isActive)
    {
      _leftDoor.SetActive(isActive);
      _rightDoor.SetActive(isActive);
    }

    void ShowTransitionDoor()
    {
      SetWallDoorsActive(false);

      var transitionDoor = GameManager.Instance.Player.IsFacingRight()
        ? _leftTransitionDoor
        : _rightTransitionDoor;
      transitionDoor.SetActive(true);
    }

    void HideTransitionDoor()
    {
      SetWallDoorsActive(true);

      _leftTransitionDoor.SetActive(false);
      _rightTransitionDoor.SetActive(false);
    }

    void OnFadeInCompleted()
    {
      GhostStoryGameContext.Instance.RegisterCallback(.25f, HideTransitionDoor, "HideTransitionDoor");
    }

    void StartCameraScroll()
    {
      var targetPosition = CameraController.CalculateTargetPosition(CameraMovementSettings);
      var cameraPosition = CameraController.Transform.position;

      var cameraTranslations = TranslateTransformActionFactory.Create(
        cameraPosition,
        targetPosition,
        FullScreenScrollerTransitionMode,
        FullScreenScrollSettings.TransitionTime,
        VerticalFullScreenScrollerTransitionSpeedFactor);

      ScrollActions = new TranslateTransformActions(cameraTranslations);

      CameraController.RegisterScrollActions(ScrollActions);
      Status = ScrollStatus.Scrolling;
    }

    void OnFadeOutCompleted()
    {
      GhostStoryGameContext.Instance.RegisterCallback(.4f, StartCameraScroll, "StartCameraScroll");
    }
  }
}