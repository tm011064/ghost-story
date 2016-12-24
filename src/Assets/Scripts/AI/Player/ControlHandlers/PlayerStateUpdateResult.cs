using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStateUpdateResult : IComparable<PlayerStateUpdateResult>
{
  public readonly static PlayerStateUpdateResult Unhandled = new PlayerStateUpdateResult();

  public bool IsHandled;

  public AnimationClipInfo AnimationClipInfo;

  public bool AllowHorizontalSpriteFlip;

  private PlayerStateUpdateResult()
  {
  }

  private PlayerStateUpdateResult(
    int animationHash,
    int animationWeight = 0,
    float animationSpeed = 1f,
    int[] linkedShortNameHashes = null,
    bool allowHorizontalSpriteFlip = true)
  {
    AnimationClipInfo = new AnimationClipInfo
    {
      ShortNameHash = animationHash,
      Weight = animationWeight,
      LinkedShortNameHashes = new HashSet<int>(linkedShortNameHashes ?? new int[0]),
      Speed = animationSpeed
    };

    IsHandled = true;
    AllowHorizontalSpriteFlip = allowHorizontalSpriteFlip;
  }

  public static PlayerStateUpdateResult CreateHandled(
    string animationName,
    int animationWeight = 0,
    float animationSpeed = 1f,
    string[] linkedAnimationNames = null,
    bool allowHorizontalSpriteFlip = true)
  {
    return new PlayerStateUpdateResult(
      Animator.StringToHash(animationName),
      animationWeight,
      animationSpeed,
      linkedAnimationNames == null
        ? null
        : linkedAnimationNames.Select(name => Animator.StringToHash(name)).ToArray(),
      allowHorizontalSpriteFlip);
  }

  public static PlayerStateUpdateResult CreateHandled(
    int animationHash,
    int animationWeight = 0,
    float animationSpeed = 1f,
    int[] linkedShortNameHashes = null,
    bool allowHorizontalSpriteFlip = true)
  {
    return new PlayerStateUpdateResult(animationHash, animationWeight, animationSpeed, linkedShortNameHashes, allowHorizontalSpriteFlip);
  }

  public static PlayerStateUpdateResult Max(
    PlayerStateUpdateResult a,
    PlayerStateUpdateResult b,
    params PlayerStateUpdateResult[] others)
  {
    PlayerStateUpdateResult max = a.CompareTo(b) > 0 ? a : b;

    if (others != null)
    {
      for (var i = 0; i < others.Length; i++)
      {
        if (others[i].CompareTo(max) > 0)
        {
          max = others[i];
        }
      }
    }

    return max;
  }

  public int CompareTo(PlayerStateUpdateResult other)
  {
    if (IsHandled)
    {
      return other.IsHandled
        ? AnimationClipInfo.Weight.CompareTo(other.AnimationClipInfo.Weight)
        : 1;
    }

    return other.IsHandled ? -1 : 0;
  }
}