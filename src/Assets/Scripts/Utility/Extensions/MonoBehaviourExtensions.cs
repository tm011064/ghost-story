using System;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions
{
  public static IEnumerable<T> FindSceneComponents<T>(this MonoBehaviour self)
    where T : class
  {
    var monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();

    for (var i = 0; i < monoBehaviours.Length; i++)
    {
      var component = monoBehaviours[i] as T;

      if (component != null)
      {
        yield return component;
      }
    }
  }

  public static GameObject GetChildGameObject(this MonoBehaviour self, string name)
  {
    return self.transform.GetChildGameObject(name);
  }

  public static GameObject GetOrCreateChildGameObject(this MonoBehaviour self, string name, string layerName = null)
  {
    var childTransform = self.transform.FindChild(name);

    if (childTransform != null)
    {
      return childTransform.gameObject;
    }

    var gameObject = new GameObject(name);

    gameObject.transform.position = self.transform.TransformPoint(Vector3.zero);
    gameObject.transform.parent = self.transform;

    if (!string.IsNullOrEmpty(layerName))
    {
      gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    return gameObject;
  }

  public static void Invoke(this MonoBehaviour self, TimeSpan delay, Action callback)
  {
    var timedAction = new TimedActionEnumerator(delay, callback);

    self.StartCoroutine(timedAction);
  }
}
