using System;
using UnityEngine;

public class GameObjectEventArgs : EventArgs
{
  public GameObjectEventArgs(GameObject gameObject)
  {
    GameObject = gameObject;
  }

  public GameObject GameObject { get; private set; }
}
