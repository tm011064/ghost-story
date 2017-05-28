using System;
using Assets.Scripts.GhostStory.Behaviours;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SaveGameArea : MonoBehaviour, IScenePortal
{
  private bool _isPlayerWithinBoundingBox;

  private bool _hasTriggeredSaveScene;

  public string PortalName;

  public string GetPortalName()
  {
    return PortalName;
  }

  public bool HasName(string name)
  {
    return string.Equals(PortalName, name, StringComparison.OrdinalIgnoreCase);
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

  void Update()
  {
    if (_isPlayerWithinBoundingBox
      && !_hasTriggeredSaveScene
      && !GameManager.Instance.InputStateManager.IsVerticalAxisHandled()
      && GameManager.Instance.InputStateManager.IsUpAxisButtonDown(GameManager.Instance.InputSettings)
      && !GameManager.Instance.SceneManager.IsFading())
    {
      GhostStoryGameContext.Instance.GameState.SpawnPlayerName = GameManager.Instance.Player.name;
      GhostStoryGameContext.Instance.GameState.SpawnPlayerPortalName = PortalName;
      GhostStoryGameContext.Instance.GameState.SpawnPlayerSceneName = SceneManager.GetActiveScene().name;

      GhostStoryGameContext.Instance.SaveGameState(GhostStoryGameContext.Instance.GameState);

      GameManager.Instance.Player.PushControlHandler(
        FreezePlayerControlHandler.CreateInvincible("Idle"));

      GameManager.Instance.SceneManager.FadeOut(OnFadeOutCompleted);

      _hasTriggeredSaveScene = true;
    }
  }

  void LoadScene()
  {
    GameManager.Instance.SceneManager.LoadScene(
      SceneManager.GetActiveScene().name,
      PortalName,
      Vector3.zero);
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

  public bool CanSpawn()
  {
    var config = GetComponent<LevelObjectConfig>();

    return GhostStoryGameContext.Instance.GameState.ActiveUniverse == config.Universe;
  }
}
