using UnityEngine;

public partial class WorldSwitchPortal : MonoBehaviour, IScenePortal
{
  private bool _isPlayerWithinBoundingBox;

  private bool _hasTriggeredSceneLoad;

  private PlayerControlHandler _realWorldFreezeControlHandler;

  public string TransitionToScene;

  public string TransitionToPortalName;

  public string PortalName;

  void Update()
  {
    if (_isPlayerWithinBoundingBox
      && !_hasTriggeredSceneLoad
      && !GameManager.Instance.InputStateManager.IsVerticalAxisHandled()
      && GameManager.Instance.InputStateManager.IsUpAxisButtonDown(GameManager.Instance.InputSettings)
      && !GameManager.Instance.SceneManager.IsFading())
    {
      Logger.UnityDebugLog("SWITCH");
      if (GhostStoryGameContext.Instance.IsAlternateWorldActivated())
      {
        SwitchToRealWorld();
      }
      else
      {
        SwitchToAlternateWorld();
      }
    }
  }

  private void SwitchToAlternateWorld()
  {
    var position = GameManager.Instance.Player.transform.position;

    _realWorldFreezeControlHandler = new FreezeRealWorldSwitchPortalControlHandler(GameManager.Instance.Player);

    GameManager.Instance.Player.PushControlHandler(_realWorldFreezeControlHandler);

    GameManager.Instance.ActivatePlayer(PlayableCharacterNames.Kino.ToString(), position);
    GameManager.Instance.Player.EnableAndShow();

    GhostStoryGameContext.Instance.SwitchToAlternateWorld();
  }

  private void SwitchToRealWorld()
  {
    var player = GameManager.Instance.GetPlayerByName(PlayableCharacterNames.Misa.ToString());
    player.transform.position = GameManager.Instance.Player.transform.position;

    GhostStoryGameContext.Instance.SwitchToRealWorld();

    GameManager.Instance.Player.DisableAndHide();

    GameManager.Instance.ActivatePlayer(
      PlayableCharacterNames.Misa.ToString(),
      GameManager.Instance.GetPlayerByName(PlayableCharacterNames.Misa.ToString()).transform.position);

    GameManager.Instance.Player.RemoveControlHandler(_realWorldFreezeControlHandler);
  }

  void LoadScene()
  {
    GameManager.Instance.SceneManager.LoadScene(TransitionToScene, TransitionToPortalName, Vector3.zero);
  }

  void OnFadeOutCompleted()
  {
    GhostStoryGameContext.Instance.RegisterCallback(.4f, LoadScene, "LoadScene");
  }

  void OnEnable()
  {
    _isPlayerWithinBoundingBox = false;
  }

  void OnDisable()
  {
    _isPlayerWithinBoundingBox = false;
  }

  void OnTriggerEnter2D(Collider2D col)
  {
    _isPlayerWithinBoundingBox = true;
  }

  void OnTriggerExit2D(Collider2D col)
  {
    _isPlayerWithinBoundingBox = false;
  }

  public string GetPortalName()
  {
    return PortalName;
  }

  public void SpawnPlayer()
  {
    GameManager.Instance.Player.transform.position = transform.position;
    GameManager.Instance.Player.CharacterPhysicsManager.WarpToFloor();
  }

  public void SpawnPlayerFromPortal(Vector3 fromPortalPosition)
  {
    SpawnPlayer();

    GameManager.Instance.SceneManager.FadeIn();

    GameManager.Instance.SceneManager.FocusCameraOnPlayer();
  }
}
