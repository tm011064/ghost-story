using UnityEngine;

public class HouseDoorFadeInBehaviour : CameraScroller
{
  private HouseDoor HouseDoor;

  private FreezePlayerControlHandler _freezeControlHandler;

  public HorizontalDirection DoorLocation;

  protected override void OnAwake()
  {
    OverrideSettings(
      GhostStoryGameContext.Instance.GameSettings.SmoothDampMoveSettings,
      GhostStoryGameContext.Instance.GameSettings.CameraSettings,
      GhostStoryGameContext.Instance.GameSettings.HorizontalCamereaWindowSettings,
      GhostStoryGameContext.Instance.GameSettings.VerticalSnapWindowSettings);

    FullScreenScrollSettings = GhostStoryGameContext.Instance.GameSettings.FullScreenScrollSettings;

    HouseDoor = GetComponentInParent<HouseDoor>();
  }

  public void StartCameraScroll(Vector3 fromPortalPosition)
  {
    ShowTransitionDoor();

    GameManager.Instance.SceneManager.ShowBlackScreen();

    var player = GameManager.Instance.Player;

    SetPlayerPosition(player);

    PushPlayerTransitionControlHandler(player);

    CameraController.Transform.position = CalculateStartCameraPosition(fromPortalPosition);

    ScrollActions = CreateScrollActions();
    CameraController.RegisterScrollActions(ScrollActions);

    Status = ScrollStatus.Scrolling;
  }

  private TranslateTransformActions CreateScrollActions()
  {
    var cameraTranslations = TranslateTransformActionFactory.Create(
      CameraController.Transform.position,
      CameraController.CalculateTargetPosition(CameraMovementSettings),
      FullScreenScrollSettings);

    return new TranslateTransformActions(cameraTranslations);
  }

  private void PushPlayerTransitionControlHandler(PlayerController player)
  {
    _freezeControlHandler = FreezePlayerControlHandler.CreateInvincible("Idle");

    player.PushControlHandler(_freezeControlHandler);
  }

  private Vector3 CalculateStartCameraPosition(Vector3 fromPortalPosition)
  {
    var doorPositionRelativeToCameraPosition = HouseDoor.transform.position - CameraController.transform.position;

    var cameraPosition = doorPositionRelativeToCameraPosition - fromPortalPosition;

    return AdjustForTransitionDoorWidth(cameraPosition)
      .SetZ(CameraController.CalculateTargetZAxisPosition());
  }

  private Vector3 AdjustForTransitionDoorWidth(Vector3 cameraPosition)
  {
    return cameraPosition.AddX(2 * HouseDoor.TransitionDoor.transform.localPosition.x);
  }

  private Vector3 CalculatePlayerTranslationVector()
  {
    var directionMultiplier = GameManager.Instance.Player.IsFacingRight() ? 1 : -1;

    return new Vector3(FullScreenScrollSettings.PlayerTranslationDistance / 2 * directionMultiplier, 0, 0);
  }

  private void SetPlayerPosition(PlayerController player)
  {
    MovePlayerToTargetPosition();

    player.transform.position = HouseDoor.transform.position.SetY(player.transform.position.y);
  }

  public void MovePlayerToTargetPosition()
  {
    var player = GameManager.Instance.Player;

    player.SetHorizontalSpriteScale(DoorLocation.Opposite());

    var playerTranslationVector = CalculatePlayerTranslationVector();

    player.transform.position = HouseDoor.transform.position + playerTranslationVector;
    player.CharacterPhysicsManager.WarpToFloor();
  }

  private void MovePlayerIntoRoom()
  {
    GameManager.Instance.Player.PushControlHandlers(
      FreezePlayerControlHandler.CreateInvincible("Idle", .4f),
      new TranslateFrozenPlayerControlHandler(
        GameManager.Instance.Player,
        .5f,
        Animator.StringToHash("Run Start"),
        CalculatePlayerTranslationVector(),
        EasingType.Linear),
      FreezePlayerControlHandler.CreateInvincible("Idle", .2f));

    GameManager.Instance.Player.RemoveControlHandler(_freezeControlHandler);
    _freezeControlHandler = null;
  }

  void FlipTransitionDoor()
  {
    var spriteRenderer = HouseDoor.TransitionDoor.GetComponent<SpriteRenderer>();

    spriteRenderer.flipX = !spriteRenderer.flipX;
  }

  void ShowTransitionDoor()
  {
    FlipTransitionDoor();

    HouseDoor.Door.SetActive(false);
    HouseDoor.TransitionDoor.SetActive(true);
  }

  void HideTransitionDoor()
  {
    HouseDoor.Door.SetActive(true);
    HouseDoor.TransitionDoor.SetActive(false);

    FlipTransitionDoor();
  }

  public void FadeIn()
  {
    OnCameraScrollCompleted();
  }

  protected override void OnCameraScrollCompleted()
  {
    MovePlayerIntoRoom();

    GhostStoryGameContext.Instance.RegisterCallback(
      .3f,
      () => GameManager.Instance.SceneManager.FadeIn(OnFadeInCompleted),
      "FadeIn");

    CameraController.Reset();

    CameraMovementSettings = CreateCameraMovementSettings(); // TODO (Roman): ???

    CameraController.OnCameraModifierEnter(CameraMovementSettings);
  }

  void OnFadeInCompleted()
  {
    GhostStoryGameContext.Instance.RegisterCallback(.25f, HideTransitionDoor, "HideTransitionDoor");
  }
}
