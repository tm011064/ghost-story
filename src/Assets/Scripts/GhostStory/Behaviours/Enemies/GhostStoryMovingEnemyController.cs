using UnityEngine;

public class GhostStoryMovingEnemyController : MovingEnemyController
{
  protected GhostStoryGameContext GhostStoryGameContext;

  protected EnemyDamageBehaviour EnemyDamageBehaviour;

  public EnemyHealthBehaviour EnemyHealthBehaviour;

  public override void Awake()
  {
    base.Awake();

    GhostStoryGameContext = GhostStoryGameContext.Instance;

    EnemyDamageBehaviour = this.GetComponentOrThrow<EnemyDamageBehaviour>();
    EnemyHealthBehaviour = this.GetComponentOrThrow<EnemyHealthBehaviour>();

    EnemyHealthBehaviour.OnHealthReduced += OnHealthReduced;
  }

  void OnDestroy()
  {
    EnemyHealthBehaviour.OnHealthReduced -= OnHealthReduced;
  }

  protected virtual void OnHealthReduced()
  {
  }

  protected bool CanCollide()
  {
    return GhostStoryGameContext.Instance.GameState.ActiveUniverse == EnemyDamageBehaviour.Universe;
  }

  protected override void OnFreeze()
  {
    // TODO (Important): this is just a quick hack, do this properly eventually
    var sprite = GetComponentInChildren<SpriteRenderer>();
    sprite.color = new Color(.3f, .3f, .3f, 1);
  }

  protected override void OnUnfreeze(float freezeDuration)
  {
    var sprite = GetComponentInChildren<SpriteRenderer>();
    sprite.color = new Color(1, 1, 1, 1);
  }
}
