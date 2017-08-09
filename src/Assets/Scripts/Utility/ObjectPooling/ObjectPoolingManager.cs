using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolingManager
{
  private static object SyncRoot = new object();

  private static ObjectPoolingManager _instance;

  private Dictionary<string, ObjectPool> _objectPools;

  public event Action<GameObject> BeforeDeactivated;

  public event Action<GameObject> AfterDeactivated;

  private ObjectPoolingManager()
  {
    _objectPools = new Dictionary<String, ObjectPool>();
  }

  public void Reset()
  {
    Logger.Info("Reset object pool.");

    DeactivateAll();

    foreach (ObjectPool objectPool in _objectPools.Values)
    {
      foreach (GameObject item in objectPool.GetGameObjects())
      {
        GameObject.Destroy(item);
      }
    }

    _objectPools = new Dictionary<string, ObjectPool>();
  }

  public static ObjectPoolingManager Instance
  {
    get
    {
      if (_instance == null)
      {
        lock (SyncRoot)
        {
          if (_instance == null)
          {
            _instance = new ObjectPoolingManager();
          }
        }
      }

      return _instance;
    }
  }

  public void RegisterPoolOrThrow(GameObject objToPool, int initialPoolSize, int maxPoolSize)
  {
    if (_objectPools.ContainsKey(objToPool.name))
    {
      throw new InvalidOperationException("Prefab " + objToPool.name + " could not be registered at pool as it already exists");
    }

    var objectPool = ObjectPool.Create(objToPool, initialPoolSize, maxPoolSize);

    _objectPools.Add(objToPool.name, objectPool);

    Logger.Info("Registered " + objToPool.name + " at object pool manager");
  }

  public void RegisterOrExpandPool(GameObject objToPool, int initialPoolSize, int maxPoolSize = -1)
  {
    ObjectPool pool;
    if (!_objectPools.TryGetValue(objToPool.name, out pool))
    {
      var objectPool = ObjectPool.Create(objToPool, initialPoolSize, maxPoolSize);

      _objectPools.Add(objToPool.name, objectPool);

      Logger.Info("Registered " + objToPool.name + " at object pool manager");

      return;
    }

    pool.Expand(initialPoolSize, maxPoolSize);

    Logger.Info("Expanded pool for " + objToPool.name);
  }

  public bool TryGetObject(string objName, Vector3 position, out GameObject gameObject)
  {
    return _objectPools[objName].TryGetObject(position, out gameObject);
  }

  public GameObject GetObject(string objName, Vector3 position)
  {
    return _objectPools[objName].GetObject(position);
  }

  public GameObject GetObject(string objName)
  {
    Logger.Assert(_objectPools.ContainsKey(objName), "Key " + objName + " not found at object pool");

    return _objectPools[objName].GetObject();
  }

  public void Deactivate(GameObject obj)
  {
    if (obj == null || !obj.activeSelf)
    {
      return;
    }

    var beforeDeactivatedHandler = BeforeDeactivated;
    if (beforeDeactivatedHandler != null)
    {
      beforeDeactivatedHandler.Invoke(obj);
    }

    obj.SendMessage("OnBeforeDisable", SendMessageOptions.DontRequireReceiver);
    obj.SetActive(false);

    var afterDeactivatedHandler = AfterDeactivated;
    if (afterDeactivatedHandler != null)
    {
      afterDeactivatedHandler.Invoke(obj);
    }
  }

  public IEnumerable<TComponent> GetAllActive<TComponent>()
  {
    return _objectPools.Values
      .SelectMany(
        pool => pool.GetGameObjects()
          .Where(o => o.activeSelf)
          .SelectMany(o => o.GetComponents<TComponent>()));
  }

  public void DeactivateAll()
  {
    Logger.Info("Deactivating all pooled objects.");

    foreach (ObjectPool objectPool in _objectPools.Values)
    {
      foreach (GameObject item in objectPool.GetGameObjects())
      {
        Deactivate(item);
      }
    }
  }
}
