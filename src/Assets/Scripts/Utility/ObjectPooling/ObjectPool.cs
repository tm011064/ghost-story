using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
  private List<GameObject> _pooledObjects = new List<GameObject>();

  private GameObject _pooledObjectPrefab;

  private int _maxPoolSize;

  private int _initialPoolSize;

  public static ObjectPool Create(GameObject prefab, int numberOfGameObjectsToInstantiate = 0, int maxNumberOfGameObjectsToInstantiate = -1)
  {
    var pool = new ObjectPool(prefab);

    pool.Expand(numberOfGameObjectsToInstantiate, maxNumberOfGameObjectsToInstantiate);

    return pool;
  }

  private ObjectPool(GameObject prefab)
  {
    _pooledObjectPrefab = prefab;
  }

  public void Expand(int numberOfGameObjectsToInstantiate, int maxNumberOfGameObjectsToInstantiate)
  {
    InstantiateObjects(_pooledObjectPrefab, numberOfGameObjectsToInstantiate, maxNumberOfGameObjectsToInstantiate);
  }

  private void InstantiateObjects(GameObject pooledObject, int numberOfGameObjectsToInstantiate, int maxNumberOfGameObjectsToInstantiate)
  {
    for (var i = 0; i < numberOfGameObjectsToInstantiate; i++)
    {
      var gameObject = Object.Instantiate(pooledObject, Vector3.zero, Quaternion.identity) as GameObject;

      gameObject.SetActive(false);
      Object.DontDestroyOnLoad(gameObject);

      _pooledObjects.Add(gameObject);
    }

    _maxPoolSize = _maxPoolSize < 0
      ? maxNumberOfGameObjectsToInstantiate
      : _maxPoolSize + maxNumberOfGameObjectsToInstantiate;

    _initialPoolSize += numberOfGameObjectsToInstantiate;
  }

  public GameObject GetObject()
  {
    return GetObject(null);
  }

  public bool TryGetObject(Vector3? position, out GameObject gameObject)
  {
    gameObject = GetObject(position);
    return gameObject != null;
  }

  public GameObject GetObject(Vector3? position)
  {
    for (var i = 0; i < _pooledObjects.Count; i++)
    {
      if (_pooledObjects[i].activeSelf == false)
      {
        if (position.HasValue)
        {
          _pooledObjects[i].transform.position = position.Value;
        }

        _pooledObjects[i].SetActive(true);

        return _pooledObjects[i];
      }
    }

    if (_maxPoolSize < 0 || _maxPoolSize > _pooledObjects.Count)
    {
      var gameObject = Object.Instantiate(_pooledObjectPrefab, position ?? Vector3.zero, Quaternion.identity) as GameObject;

      gameObject.SetActive(true);

      _pooledObjects.Add(gameObject);

      return gameObject;
    }

    return null;
  }

  public IEnumerable<GameObject> GetGameObjects()
  {
    for (var i = 0; i < _pooledObjects.Count; i++)
    {
      yield return _pooledObjects[i];
    }
  }

  public void Shrink()
  {
    int objectsToRemoveCount = _pooledObjects.Count - _initialPoolSize;

    if (objectsToRemoveCount <= 0)
    {
      return;
    }

    for (var i = _pooledObjects.Count - 1; i >= 0; i--)
    {
      if (!_pooledObjects[i].activeSelf)
      {
        var gameObject = _pooledObjects[i];

        _pooledObjects.Remove(gameObject);
      }
    }
  }
}
