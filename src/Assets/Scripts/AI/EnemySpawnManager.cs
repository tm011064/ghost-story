using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EnemySpawnManager : SpawnBucketItemBehaviour, IObjectPoolBehaviour, IFreezable
{
  public RespawnMode RespawnMode = RespawnMode.SpawnOnce;

  public bool DestroySpawnedEnemiesWhenGettingDisabled = false;

  public float ContinuousSpawnInterval = 10f;

  [Range(1f / 30.0f, float.MaxValue)]
  public float RespawnOnDestroyDelay = .1f;

  public SpawnArgument[] SpawnArguments;

  private readonly List<GameObject> _spawnedEnemies = new List<GameObject>();

  private ObjectPoolingManager _objectPoolingManager;

  private bool _isDisabling;

  private GameObject _enemyToSpawnPrefab;

  private ISpawnable _spawnablePrefabComponent;

  private bool _isEnemyToSpawnPrefabLoaded;

  private GhostStoryGameContext _gameContext;

  void Awake()
  {
    if (!_isEnemyToSpawnPrefabLoaded)
    {
      LoadEnemyToSpawnPrefab();
    }

    _gameContext = GhostStoryGameContext.Instance;
  }

  private void LoadEnemyToSpawnPrefab()
  {
    _spawnablePrefabComponent = gameObject.GetComponentInChildren<ISpawnable>(true);

    if (_spawnablePrefabComponent == null)
    {
      throw new MissingReferenceException("Enemy spawn manager '" + name
        + "' does not contain a child object that inmplements an ISpawnable interface");
    }

    _enemyToSpawnPrefab = (_spawnablePrefabComponent as MonoBehaviour).gameObject;

    _enemyToSpawnPrefab.SetActive(false);

    _isEnemyToSpawnPrefabLoaded = true;
  }

  private void Spawn()
  {
    _enemyToSpawnPrefab.transform.position = transform.position;

    if (!_spawnablePrefabComponent.CanSpawn())
    {
      ScheduleSpawn();

      return;
    }

    var spawnedEnemy = _objectPoolingManager.GetObject(_enemyToSpawnPrefab.name, transform.position);

    spawnedEnemy.transform.localScale = _enemyToSpawnPrefab.transform.localScale;

    var spawnable = spawnedEnemy.GetComponent<ISpawnable>();

    spawnable.Reset(SpawnArguments.ToDictionary(c => c.Key, c => c.Value));

    spawnable.GotDisabled += OnEnemyControllerGotDisabled;

    _spawnedEnemies.Add(spawnedEnemy);

    ScheduleNextSpawn();
  }

  private void ScheduleSpawn()
  {
    try
    {
      if (isActiveAndEnabled
        && RespawnMode == RespawnMode.SpawnWhenDestroyed)
      {
        _gameContext.RegisterCallback(RespawnOnDestroyDelay, Spawn, "Spawn");
      }
    }
    catch (MissingReferenceException)
    {
      // we swallow that one, it happens on scene unload when an enemy disables after this object has been finalized
    }
  }

  void OnEnemyControllerGotDisabled(BaseMonoBehaviour obj)
  {
    obj.GotDisabled -= OnEnemyControllerGotDisabled; // unsubscribed cause this could belong to a pooled object

    if (!_isDisabling)
    {
      // while we are disabling this object we don't want to touch the spawned enemies list nor respawn
      _spawnedEnemies.Remove(obj.gameObject);

      ScheduleSpawn();
    }
  }

  private void ScheduleNextSpawn()
  {
    if (RespawnMode == RespawnMode.SpawnContinuously
        && ContinuousSpawnInterval > 0f)
    {
      _gameContext.RegisterCallback(ContinuousSpawnInterval, Spawn, "Spawn");
    }
  }
  
  public void DeactivateSpawnedObjects()
  {
    _isDisabling = true;

    for (var i = _spawnedEnemies.Count - 1; i >= 0; i--)
    {
      _objectPoolingManager.Deactivate(_spawnedEnemies[i]);

      _spawnedEnemies.RemoveAt(i);
    }

    _isDisabling = false;
  }

  void OnDisable()
  {
    Logger.Trace("Disabling EnemySpawnManager {0}", name);

    if (DestroySpawnedEnemiesWhenGettingDisabled)
    {
      _isDisabling = true;

      for (var i = _spawnedEnemies.Count - 1; i >= 0; i--)
      {
        _objectPoolingManager.Deactivate(_spawnedEnemies[i]);

        _spawnedEnemies.RemoveAt(i);
      }

      _isDisabling = false;
    }

    _gameContext.CancelCallback("Spawn");
  }

  void OnEnable()
  {
    Logger.Trace("Enabling EnemySpawnManager {0}", name);

    _objectPoolingManager = ObjectPoolingManager.Instance;

    _gameContext.RegisterCallback(0f, Spawn, "Spawn");
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    if (!_isEnemyToSpawnPrefabLoaded)
    {
      LoadEnemyToSpawnPrefab();
    }

    return GetObjectPoolRegistrationInfos(_enemyToSpawnPrefab);
  }

  public void Freeze()
  {
    foreach (var spawned in _spawnedEnemies)
    {
      var spawnable = spawned.GetComponent<ISpawnable>();
      spawnable.Freeze();
    }
  }

  public void Unfreeze()
  {
    foreach (var spawned in _spawnedEnemies)
    {
      var spawnable = spawned.GetComponent<ISpawnable>();
      spawnable.Unfreeze();
    }
  }
}
