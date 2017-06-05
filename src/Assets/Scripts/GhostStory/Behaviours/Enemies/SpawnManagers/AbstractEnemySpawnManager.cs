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

  private GameObject _enemyToSpawnPrefab;

  private ISpawnable _spawnablePrefabComponent;

  private bool _isEnemyToSpawnPrefabLoaded;

  protected string SpawnTimerName = GhostStoryGameContext.CreateUniqueCallbackName(
    typeof(AbstractEnemySpawnManager), "Spawn");

  private AbstractVisibilityCheckManager _visibilityCheckManager;

  void Awake()
  {
    if (!_isEnemyToSpawnPrefabLoaded)
    {
      LoadEnemyToSpawnPrefab();
    }
  }

  private void OnDestroy()
  {
    if (_visibilityCheckManager != null)
    {
      _visibilityCheckManager.Dispose();
      _visibilityCheckManager = null;
    }
  }

  private void LoadEnemyToSpawnPrefab()
  {
    _spawnablePrefabComponent = gameObject.GetComponentInChildren<ISpawnable>(true);

    if (_spawnablePrefabComponent == null)
    {
      throw new MissingReferenceException(
        "Enemy spawn manager '" + name + "' does not contain a child object that inmplements an ISpawnable interface");
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

    spawnable.GotDisabled += OnSpawnableDisabled;

    SpawnedEnemies.Add(spawnedEnemy);

    OnSpawnCompleted();
  }

  private void OnSpawnableDisabled(object sender, GameObjectEventArgs args)
  {
    UnsubscribeSpawnableDisabled(args.GameObject);

    SpawnedEnemies.Remove(args.GameObject);

    OnEnemyDisabled();
  }

  private void UnsubscribeSpawnableDisabled(GameObject obj)
  {
    var spawnable = obj.GetComponent<ISpawnable>();

    spawnable.GotDisabled -= OnSpawnableDisabled;
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
      GhostStoryGameContext.Instance.RegisterCallback(0, Spawn, this.GetGameObjectUniverse(), SpawnTimerName);
    }

    if (SpawnWhenBecameVisible)
    {
      StartVisibilityChecks();
    }
  }

  protected void StartVisibilityChecks()
  {
    if (_visibilityCheckManager != null)
    {
      _visibilityCheckManager.Dispose();
    }

    var collider = this.GetComponentOrThrow<Collider2D>();

    _visibilityCheckManager = ColliderVisibilityCheckManager.Create(
      collider,
      this.GetGameObjectUniverse(),
      OnGotVisible);

    _visibilityCheckManager.StartChecks();
  }

  protected void StopVisibilityChecks()
  {
    if (_visibilityCheckManager != null)
    {
      _visibilityCheckManager.StopChecks();
    }
  }

  private void OnGotVisible()
  {
    if (SpawnedEnemies.Any())
    {
      return;
    }

    GhostStoryGameContext.Instance.RegisterCallback(0, Spawn, this.GetGameObjectUniverse(), SpawnTimerName);
  }

  protected void OnDisable()
  {
    Logger.Trace("Disabling EnemySpawnManager {0}", name);

    if (DestroySpawnedEnemiesWhenGettingDisabled)
    {
      for (var i = SpawnedEnemies.Count - 1; i >= 0; i--)
      {
        UnsubscribeSpawnableDisabled(SpawnedEnemies[i]);

        ObjectPoolingManager.Instance.Deactivate(SpawnedEnemies[i]);

        SpawnedEnemies.RemoveAt(i);
      }
    }

    GhostStoryGameContext.Instance.CancelCallback(this.GetGameObjectUniverse(), SpawnTimerName);
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
