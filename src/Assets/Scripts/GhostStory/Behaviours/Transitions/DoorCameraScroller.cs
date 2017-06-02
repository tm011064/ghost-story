using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class DoorCameraScroller : CameraScroller
  {
    private GameObject _leftDoor;

    private GameObject _rightDoor;

    private GameObject _leftTransitionDoor;

    private GameObject _rightTransitionDoor;

    private BaseControlHandler _freezeControlHandler;

    private DoorTriggerEnterBehaviour _doorTriggerEnterBehaviour;
    
    protected override void OnAwake()
    {
      FullScreenScrollSettings = GhostStoryGameContext.Instance.GameSettings.FullScreenScrollSettings;
      CameraSettings = GhostStoryGameContext.Instance.GameSettings.CameraSettings;
      SmoothDampMoveSettings = GhostStoryGameContext.Instance.GameSettings.SmoothDampMoveSettings;
      VerticalSnapWindowSettings = GhostStoryGameContext.Instance.GameSettings.VerticalSnapWindowSettings;
      HorizontalCamereaWindowSettings = GhostStoryGameContext.Instance.GameSettings.HorizontalCamereaWindowSettings;

      CameraMovementSettings = CreateCameraMovementSettings();

      _doorTriggerEnterBehaviour = GetComponentInChildren<DoorTriggerEnterBehaviour>();
      _doorTriggerEnterBehaviour.Open += TriggerScroll;

      _leftDoor = gameObject.transform.parent.Find("Left Door").gameObject;
      _rightDoor = gameObject.transform.parent.Find("Right Door").gameObject;
      _leftTransitionDoor = gameObject.transform.parent.Find("Left Transition Door").gameObject;
      _rightTransitionDoor = gameObject.transform.parent.Find("Right Transition Door").gameObject;
    }

    protected override void OnDestroy()
    {
      _doorTriggerEnterBehaviour.Open -= TriggerScroll;

      base.OnDestroy();
    }

    private void MovePlayerIntoDoor()
    {
      var directionMultiplier = GameManager.Instance.Player.IsFacingRight() ? 1 : -1;

      _freezeControlHandler = FreezePlayerControlHandler.CreateInvincible("Idle");

      GameManager.Instance.Player.PushControlHandlers(
        _freezeControlHandler,
        new TranslateFrozenPlayerControlHandler(
          GameManager.Instance.Player,
          .6f,
          Animator.StringToHash("Run Start"),
          new Vector3(FullScreenScrollSettings.PlayerTranslationDistance / 2 * directionMultiplier, 0, 0),
          EasingType.Linear),
        FreezePlayerControlHandler.CreateInvincible("Idle", .2f));
    }

    private void MovePlayerIntoRoom()
    {
      var directionMultiplier = GameManager.Instance.Player.IsFacingRight() ? 1 : -1;

      GameManager.Instance.Player.PushControlHandlers(
        FreezePlayerControlHandler.CreateInvincible("Idle", .4f),
        new TranslateFrozenPlayerControlHandler(
          GameManager.Instance.Player,
          .5f,
          Animator.StringToHash("Run Start"),
          new Vector3(FullScreenScrollSettings.PlayerTranslationDistance / 2 * directionMultiplier, 0, 0),
          EasingType.Linear),
        FreezePlayerControlHandler.CreateInvincible("Idle", .2f));

      GameManager.Instance.Player.RemoveControlHandler(_freezeControlHandler);
    }

    protected override void OnCameraScrollCompleted()
    {
      MovePlayerIntoRoom();

      GhostStoryGameContext.Instance.RegisterCallback(
        .3f,
        () => GameManager.Instance.SceneManager.FadeIn(OnFadeInCompleted),
        this.GetGameObjectUniverse());
    }

    public void TriggerScroll()
    {
      if (IsTransitionDoorActive())
      {
        return;
      }

      MovePlayerIntoDoor();

      GhostStoryGameContext.Instance.RegisterCallback(
        .8f,
        () => GameManager.Instance.SceneManager.FadeOut(() => OnFadeOutCompleted()),
        this.GetGameObjectUniverse());

      ShowTransitionDoor();
    }

    private void SetWallDoorsActive(bool isActive)
    {
      _leftDoor.SetActive(isActive);
      _rightDoor.SetActive(isActive);
    }

    bool IsTransitionDoorActive()
    {
      return _leftTransitionDoor.activeSelf || _rightTransitionDoor.activeSelf;
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
      GhostStoryGameContext.Instance.RegisterCallback(.25f, HideTransitionDoor, this.GetGameObjectUniverse());
    }

    void StartCameraScroll()
    {
      CameraController.Reset();

      var targetPosition = CameraController.CalculateTargetPosition(CameraMovementSettings);
      var cameraPosition = CameraController.Transform.position;

      var cameraTranslations = TranslateTransformActionFactory.Create(
        cameraPosition,
        targetPosition,
        FullScreenScrollSettings);

      ScrollActions = new TranslateTransformActions(cameraTranslations);

      CameraController.RegisterScrollActions(ScrollActions);
      Status = ScrollStatus.Scrolling;
    }

    void OnFadeOutCompleted()
    {
      GhostStoryGameContext.Instance.RegisterCallback(.4f, StartCameraScroll, this.GetGameObjectUniverse());
    }
  }
}
