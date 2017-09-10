using System;
using UnityEngine;

public class AttachPlayerControllerToObject : MonoBehaviour, IAttachableObject
{
  public event Action PlayerControllerGotGrounded;

  public event Action<IAttachableObject, GameObject> Attached;

  public event Action<IAttachableObject, GameObject> Detached;

  void OnPlayerGroundedPlatformChanged(GroundedPlatformArgs e)
  {
    Logger.UnityDebugLog("OnPlayerGroundedPlatformChanged");

    if (e.PreviousPlatform == gameObject
      && GameManager.Instance.Player.transform.parent == gameObject.transform)
    {
      GameManager.Instance.Player.transform.parent = null;

      var handler = Detached;

      if (handler != null)
      {
        handler.Invoke(this, gameObject);
      }

      Logger.Info("Removed parent (" + gameObject.name + " [ " + GetHashCode() + " ]) relationship from child (" + GameManager.Instance.Player.name + ") [1]");
    }
    else
    {
      if (e.CurrentPlatform == gameObject
        && GameManager.Instance.Player.transform.parent != gameObject.transform)
      {
        if (GameManager.Instance.Player.transform.parent != null)
        {
          GameManager.Instance.Player.transform.parent = null;

          Logger.Info("Removed parent (" + gameObject.name + " [ " + GetHashCode() + " ]) relationship from child (" + GameManager.Instance.Player.name + ") [2]");
        }

        GameManager.Instance.Player.transform.parent = gameObject.transform;

        Logger.Info("Added parent (" + gameObject.name + " [ " + GetHashCode() + " ]) relationship to child (" + GameManager.Instance.Player.name + ")");

        var playerControllerGotGroundedHandler = PlayerControllerGotGrounded;

        if (playerControllerGotGroundedHandler != null)
        {
          playerControllerGotGroundedHandler.Invoke();
        }

        var attachedHandler = Attached;

        if (attachedHandler != null)
        {
          attachedHandler.Invoke(this, gameObject);
        }
      }
    }
  }

  void OnBeforeDisable()
  {
    if (GameManager.Instance.Player != null
      && GameManager.Instance.Player.transform.parent == gameObject.transform)
    {
      GameManager.Instance.Player.transform.parent = null;
    }
  }

  void Start()
  {
    GameManager.Instance.Player.GroundedPlatformChanged += OnPlayerGroundedPlatformChanged;
  }

  void OnDestroy()
  {
    GameManager.Instance.Player.GroundedPlatformChanged -= OnPlayerGroundedPlatformChanged;
  }
}
