using System;
using System.Linq;
using UnityEngine;

public abstract class ColliderTriggerEnterBehaviour : MonoBehaviour
{
  private PlayerColliderState _playerColliderState;
  
  public event EventHandler<TriggerEnterExitEventArgs> Entered;

  public event EventHandler<TriggerEnterExitEventArgs> Exited;

  protected abstract void InvokeHandler(EventHandler<TriggerEnterExitEventArgs> handler, Collider2D collider);

  protected abstract bool CanEnter();

  private void InvokeEnter(Collider2D collider)
  {
    var handler = Entered;

    if (handler != null)
    {
      InvokeHandler(handler, collider);
    }
  }

  void OnTriggerStay2D(Collider2D collider)
  {
    if ((_playerColliderState & PlayerColliderState.EntryDenied) == 0)
    {
      return;
    }

    if (CanEnter())
    {
      _playerColliderState &= ~PlayerColliderState.EntryDenied;
      InvokeEnter(collider);
    }
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    _playerColliderState = PlayerColliderState.Inside;

    if (!CanEnter())
    {
      _playerColliderState |= PlayerColliderState.EntryDenied;
      return;
    }

    InvokeEnter(collider);
  }

  void OnTriggerExit2D(Collider2D collider)
  {
    _playerColliderState = PlayerColliderState.Outside;

    var handler = Exited;

    if (handler != null)
    {
      InvokeHandler(handler, collider);
    }
  }

  private enum PlayerColliderState
  {
    Outside,
    Inside,
    EntryDenied
  }
}
