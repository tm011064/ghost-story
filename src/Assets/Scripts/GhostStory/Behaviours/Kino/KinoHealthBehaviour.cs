public class KinoHealthBehaviour : PlayerHealthBehaviour
{
  void Awake()
  {
    GhostStoryGameContext.Instance.GameStateChanged += OnGameStateChanged;
  }

  void OnDestroy()
  {
    GhostStoryGameContext.Instance.GameStateChanged -= OnGameStateChanged;
  }

  void OnGameStateChanged(GhostStoryGameState gameState)
  {
    SetHealthUnits(gameState.KinoHealthUnits);
  }
}
