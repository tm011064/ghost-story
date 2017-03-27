using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TranslateTransformActionsManager
{
  private readonly Queue<TranslateTransformActions> _translateTransformActions =
    new Queue<TranslateTransformActions>(8);

  public event Action<TranslateTransformActions> Completed;

  public void EnqueueScrollActions(TranslateTransformActions translateTransformActions)
  {
    _translateTransformActions.Enqueue(translateTransformActions);
  }

  public Vector3 GetPosition()
  {
    return _translateTransformActions.Peek().GetPosition();
  }

  public bool HasActions()
  {
    return _translateTransformActions.Any();
  }

  public Vector3 CalculatePosition(Vector3 position)
  {
    var activeActions = _translateTransformActions.Peek();
    var updatedPosition = activeActions.Update(position);

    if (!activeActions.HasAction())
    {
      var handler = Completed;
      if (handler != null)
      {
        handler(activeActions);
      }

      _translateTransformActions.Dequeue();
      if (_translateTransformActions.Any())
      {
        return CalculatePosition(updatedPosition);
      }
    }

    return updatedPosition;
  }
}
