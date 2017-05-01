using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract partial class AbstractEnemySpawnManager : SpawnBucketItemBehaviour, IObjectPoolBehaviour, IFreezable
{
  public bool DestroySpawnedEnemiesWhenGettingDisabled = false;

  public SpawnArgument[] SpawnArguments;

  protected readonly List<GameObject> SpawnedEnemies = new List<GameObject>();

  public bool SpawnWhenBecameVisible = true;

  public bool SpawnOnSceneLoad;

  private bool _isDisabling;

  private GameObject _enemyToSpawnPrefab;

  private ISpawnable _spawnablePrefabComponent;

  private bool _isEnemyToSpawnPrefabLoaded;

  void Awake()
  {
    if (!_isEnemyToSpawnPrefabLoaded)
    {
      LoadEnemyToSpawnPrefab();
    }
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

  protected void Spawn()
  {
    _enemyToSpawnPrefab.transform.position = transform.position;

    if (!_spawnablePrefabComponent.CanSpawn())
    {
      OnSpawnFailed();
      return;
    }

    var spawnedEnemy = ObjectPoolingManager.Instance.GetObject(_enemyToSpawnPrefab.name, transform.position);

    spawnedEnemy.transform.localScale = _enemyToSpawnPrefab.transform.localScale;

    var spawnable = spawnedEnemy.GetComponent<ISpawnable>();

    spawnable.Reset(SpawnArguments.ToDictionary(c => c.Key, c => c.Value));

    spawnable.GotDisabled += OnEnemyControllerGotDisabled;

    SpawnedEnemies.Add(spawnedEnemy);

    OnSpawnCompleted();
  }

  protected virtual void OnSpawnCompleted()
  {
  }

  protected virtual void OnSpawnFailed()
  {
  }

  protected virtual void OnEnemyDisabled()
  {
  }

  void Start()
  {
    if (SpawnOnSceneLoad)
    {
      GhostStoryGameContext.Instance.RegisterCallback(0, Spawn, "Spawn");
    }

    if (SpawnWhenBecameVisible)
    {
      var collider = this.GetComponentOrThrow<Collider2D>();
      StartVisibilityChecks(.1f, collider);
    }
  }

  protected override void OnGotVisible()
  {
    if (SpawnedEnemies.Any())
    {
      return;
    }

    GhostStoryGameContext.Instance.RegisterCallback(0, Spawn, "Spawn");
  }

  void OnEnemyControllerGotDisabled(BaseMonoBehaviour obj)
  {
    obj.GotDisabled -= OnEnemyControllerGotDisabled;

    if (!_isDisabling)
    {
      SpawnedEnemies.Remove(obj.gameObject);

      OnEnemyDisabled();
    }
  }

  public void DeactivateSpawnedObjects()
  {
    _isDisabling = true;

    for (var i = SpawnedEnemies.Count - 1; i >= 0; i--)
    {
      ObjectPoolingManager.Instance.Deactivate(SpawnedEnemies[i]);

      SpawnedEnemies.RemoveAt(i);
    }

    _isDisabling = false;
  }

  protected override void OnDisable()
  {
    Logger.Trace("Disabling EnemySpawnManager {0}", name);

    if (DestroySpawnedEnemiesWhenGettingDisabled)
    {
      _isDisabling = true;

      for (var i = SpawnedEnemies.Count - 1; i >= 0; i--)
      {
        ObjectPoolingManager.Instance.Deactivate(SpawnedEnemies[i]);

        SpawnedEnemies.RemoveAt(i);
      }

      _isDisabling = false;
    }

    GhostStoryGameContext.Instance.CancelCallback("Spawn");

    base.OnDisable();
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
    foreach (var spawned in SpawnedEnemies)
    {
      var spawnable = spawned.GetComponent<ISpawnable>();
      spawnable.Freeze();
    }
  }

  public void Unfreeze()
  {
    foreach (var spawned in SpawnedEnemies)
    {
      var spawnable = spawned.GetComponent<ISpawnable>();
      spawnable.Unfreeze();
    }
  }
}
