﻿using System;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class FrontPortal : MonoBehaviour, IScenePortal
  {
    private bool _isPlayerWithinBoundingBox;

    private bool _hasTriggeredSceneLoad;

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
        GameManager.Instance.Player.PushControlHandler(
          FreezePlayerControlHandler.CreateInvincible("Idle"));

        GameManager.Instance.SceneManager.FadeOut(OnFadeOutCompleted);

        _hasTriggeredSceneLoad = true;
      }
    }

    void LoadScene()
    {
      GameManager.Instance.SceneManager.LoadScene(TransitionToScene, TransitionToPortalName, Vector3.zero);
    }

    void OnFadeOutCompleted()
    {
      GhostStoryGameContext.Instance.RegisterCallback(.4f, LoadScene, this.GetGameObjectUniverse());
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

    public bool CanSpawn()
    {
      return GhostStoryGameContext.Instance.GameState.ActiveUniverse == this.GetGameObjectUniverse();
    }
  }
}
