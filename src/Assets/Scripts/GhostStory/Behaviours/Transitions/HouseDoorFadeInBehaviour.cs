using UnityEngine;

public class HouseDoorFadeInBehaviour : CameraScroller
{
  private HouseDoor HouseDoor;

  private FreezePlayerControlHandler _freezeControlHandler;

  public HorizontalDirection DoorLocation;

  protected override void OnAwake()
  {
    FullScreenScrollSettings = GhostStoryGameContext.Instance.GameSettings.FullScreenScrollSettings;
    CameraSettings = GhostStoryGameContext.Instance.GameSettings.CameraSettings;
    SmoothDampMoveSettings = GhostStoryGameContext.Instance.GameSettings.SmoothDampMoveSettings;

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
    _freezeControlHandler = new FreezePlayerControlHandler(
        player,
        -1,
        Animator.StringToHash("Idle"),
        new PlayerState[] { PlayerState.Invincible, PlayerState.Locked });

    player.PushControlHandler(_freezeControlHandler);
  }

  private Vector3 CalculateStartCameraPosition(Vector3 fromPortalPosition)
  {
    var doorPositionRelativeToCameraPosition = HouseDoor.transform.position - CameraController.transform.position;

    var cameraPosition = doorPositionRelativeToCameraPosition - fromPortalPosition;

    return AdjustForTransitionDoorWidth(cameraPosition);
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
    AdjustPlayerSpriteScale(player);

    var playerTranslationVector = CalculatePlayerTranslationVector();

    player.transform.position = HouseDoor.transform.position + playerTranslationVector;
    player.CharacterPhysicsManager.WarpToFloor();

    player.transform.position = HouseDoor.transform.position.SetY(player.transform.position.y);
  }

  private void AdjustPlayerSpriteScale(PlayerController player)
  {
    if (DoorLocation == HorizontalDirection.Right)
    {
      player.FlipHorizontalSpriteScale();
    }
  }

  private void MovePlayerIntoRoom()
  {
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
        CalculatePlayerTranslationVector(),
        EasingType.Linear),
      new FreezePlayerControlHandler(
        GameManager.Instance.Player,
        .2f,
        Animator.StringToHash("Idle"),
        new PlayerState[] { PlayerState.Invincible, PlayerState.Locked }));

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

    CameraController.ClearCameraModifiers();
    CameraController.OnCameraModifierEnter(CameraMovementSettings);
  }

  void OnFadeInCompleted()
  {
    GhostStoryGameContext.Instance.RegisterCallback(.25f, HideTransitionDoor, "HideTransitionDoor");
  }
}
