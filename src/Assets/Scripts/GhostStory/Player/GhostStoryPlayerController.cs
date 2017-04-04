public class GhostStoryPlayerController : PlayerController
{
  protected override void OnAwake()
  {
    base.OnAwake();

    GhostStoryGameContext.Instance.GameStateChanged += OnGameStateChanged;
  }

  void OnDestroy()
  {
    GhostStoryGameContext.Instance.GameStateChanged -= OnGameStateChanged;
  }

  private void OnGameStateChanged(GhostStoryGameState gameState)
  {
    foreach (var weapon in Weapons)
    {
      weapon.gameObject.SetActive(
        gameState.GetWeapon(weapon.Name).IsActive);
    }
  }
}
