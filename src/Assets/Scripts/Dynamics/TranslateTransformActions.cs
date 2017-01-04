using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TranslateTransformActions
{
  private readonly Queue<TranslateTransformAction> _actionQueue;

  public TranslateTransformActions(IEnumerable<TranslateTransformAction> translations)
  {
    _actionQueue = new Queue<TranslateTransformAction>(translations);
  }

  public bool HasAction()
  {
    return _actionQueue.Any();
  }

  public Vector3 GetPosition()
  {
    return _actionQueue.Peek().GetPosition();
  }

  public Vector3 Update(Vector3 startPosition)
  {
    if (!_actionQueue.Any())
    {
      return startPosition;
    }

    var activeAction = _actionQueue.Peek();
    if (!activeAction.IsStarted())
    {
      activeAction.Start(startPosition);
    }

    if (activeAction.IsCompleted())
    {
      _actionQueue.Dequeue();

      return Update(activeAction.GetPosition());
    }

    return activeAction.GetPosition();
  }
}
