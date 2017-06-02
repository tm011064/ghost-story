using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreezableTimer : MonoBehaviour
{
  public float FreezeStartTime;

  public float FreezeEndTime;

  public float FreezeDuration;

  public bool IsFrozen;

  public string Name;

  private readonly List<CallbackInfo> _callbacks = new List<CallbackInfo>();

  public void Reset()
  {
    FreezeStartTime = 0f;
    FreezeEndTime = 0f;
    FreezeDuration = 0f;
    IsFrozen = false;

    _callbacks.Clear();
  }

  public void Freeze()
  {
    if (IsFrozen)
    {
      return;
    }

    IsFrozen = true;
    FreezeStartTime = Time.time;
  }

  public void Unfreeze()
  {
    if (!IsFrozen)
    {
      return;
    }

    FreezeEndTime = Time.time;
    FreezeDuration = FreezeEndTime - FreezeStartTime;

    foreach (var callback in _callbacks)
    {
      callback.CallbackTime += FreezeDuration;
    }

    IsFrozen = false;
  }

  public void RegisterCallback(float delay, Action action, string name)
  {
    var callbackInfo = new CallbackInfo
    {
      Action = action,
      CallbackTime = Time.time + delay,
      Name = name
    };

    _callbacks.Add(callbackInfo);
    _callbacks.Sort((a, b) => a.CallbackTime.CompareTo(b.CallbackTime));
  }

  public void CancelCallback(string name)
  {
    for (var i = _callbacks.Count - 1; i >= 0; i--)
    {
      if (_callbacks[i].Name == name)
      {
        _callbacks.RemoveAt(i);
      }
    }
  }

  void Update()
  {
    if (IsFrozen)
    {
      return;
    }

    while (_callbacks.Count > 0)
    {
      var callback = _callbacks.First();

      if (callback.CallbackTime > Time.time)
      {
        break;
      }

      callback.Action();

      _callbacks.Remove(callback);
    }
  }

  private class CallbackInfo
  {
    public Action Action;

    public float CallbackTime;

    public string Name;
  }
}