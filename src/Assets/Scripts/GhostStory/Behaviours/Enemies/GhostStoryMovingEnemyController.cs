using UnityEngine;

public class GhostStoryMovingEnemyController : MovingEnemyController
{
  protected GhostStoryGameContext GhostStoryGameContext;

  protected EnemyDamageBehaviour EnemyDamageBehaviour;

  protected override void OnAwake()
  {
    GhostStoryGameContext = GhostStoryGameContext.Instance;

    EnemyDamageBehaviour = this.GetComponentOrThrow<EnemyDamageBehaviour>();
  }

  protected bool CanCollide()
  {
    return GhostStoryGameContext.Instance.GameState.ActiveUniverse.Universe == EnemyDamageBehaviour.Universe;
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
