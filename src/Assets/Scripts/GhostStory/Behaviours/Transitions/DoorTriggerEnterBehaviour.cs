using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GhostStory.Behaviours.Transitions
{
  public partial class DoorTriggerEnterBehaviour : MonoBehaviour
  {
    public DoorKey[] DoorKeysNeededToEnter;

    public Direction DoorLocation;

    private bool _isInsideTriggerBounds;

    public event Action Open;

    void OnTriggerEnter2D(Collider2D collider)
    {
      _isInsideTriggerBounds = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
      _isInsideTriggerBounds = false;
    }

    void Update()
    {
      if (_isInsideTriggerBounds
        && IsPlayerFacingDoor()
        && GameManager.Instance.Player.IsGrounded()
        && GameManager.Instance.InputStateManager.IsUnhandledButtonDown("Attack")
        && DoorKeysNeededToEnter.All(x => GhostStoryGameContext.Instance.GameState.GetDoorKey(x).IsActive))
      {
        GameManager.Instance.InputStateManager.SetButtonHandled("Attack");

        var handler = Open;
        if (handler != null)
        {
          handler();
        }
      }
    }

    private bool IsPlayerFacingDoor()
    {
      switch (DoorLocation)
      {
        case Direction.Left:
          return !GameManager.Instance.Player.IsFacingRight();

        case Direction.Right:
          return GameManager.Instance.Player.IsFacingRight();
      }

      throw new NotImplementedException(DoorLocation.ToString());
    }
  }
}
