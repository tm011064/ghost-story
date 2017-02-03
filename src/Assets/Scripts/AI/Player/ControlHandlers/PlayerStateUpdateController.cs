using System.Linq;

public class PlayerStateUpdateController
{
  private readonly PlayerController _playerController;

  private readonly AbstractPlayerStateControllerSet[] _playerStateControllerSets;

  public PlayerStateUpdateController(
    PlayerController playerController,
    AbstractPlayerStateControllerSet[] playerStateControllerSets)
  {
    _playerController = playerController;
    _playerStateControllerSets = playerStateControllerSets;
  }

  public void UpdatePlayerState(XYAxisState axisState)
  {
    var results = _playerStateControllerSets
      .Select(p => p.Update(axisState))
      .ToArray();

    var playerStateUpdateResult = PlayerStateUpdateResult.Max(results);

    if ((_playerController.PlayerState & PlayerState.Locked) == 0)
    {
      AdjustSpriteScale(axisState);
    }

    if (playerStateUpdateResult.AnimationClipInfo != null)
    {
      PlayAnimation(playerStateUpdateResult.AnimationClipInfo);
    }
  }

  private void PlayAnimation(AnimationClipInfo animationClipInfo)
  {
    _playerController.Animator.speed = animationClipInfo.Speed;

    var animatorStateInfo = _playerController.Animator.GetCurrentAnimatorStateInfo(animationClipInfo.LayerIndex);

    if (animatorStateInfo.shortNameHash == animationClipInfo.ShortNameHash)
    {
      return;
    }

    if (animationClipInfo.LinkedShortNameHashes.Contains(animatorStateInfo.shortNameHash))
    {
      return;
    }

    Logger.Info("Start playing new animation clip: " + AnimationHashLookup.GetName(animationClipInfo.ShortNameHash)
      + " [" + animationClipInfo.ShortNameHash + "]");

    _playerController.Animator.Play(animationClipInfo.ShortNameHash);
  }

  private void AdjustSpriteScale(XYAxisState axisState)
  {
    if ((axisState.XAxis > 0f && _playerController.Sprite.transform.localScale.x < 1f)
      || (axisState.XAxis < 0f && _playerController.Sprite.transform.localScale.x > -1f))
    {
      _playerController.FlipHorizontalSpriteScale();
    }
  }

  public void Dispose()
  {
    for (var i = 0; i < _playerStateControllerSets.Length; i++)
    {
      _playerStateControllerSets[i].Dispose();
    }
  }

}