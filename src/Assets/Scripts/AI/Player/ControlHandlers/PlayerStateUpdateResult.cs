using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStateUpdateResult : IComparable<PlayerStateUpdateResult>
{
  public readonly static PlayerStateUpdateResult Unhandled = new PlayerStateUpdateResult();

  public bool IsHandled;

  public AnimationClipInfo AnimationClipInfo;

  private PlayerStateUpdateResult()
  {
  }

  private PlayerStateUpdateResult(
    int animationHash,
    int animationWeight = 0,
    float animationSpeed = 1f,
    int[] linkedShortNameHashes = null,
    int layerIndex = 0)
  {
    AnimationClipInfo = new AnimationClipInfo
    {
      ShortNameHash = animationHash,
      Weight = animationWeight,
      LinkedShortNameHashes = new HashSet<int>(linkedShortNameHashes ?? new int[0]),
      Speed = animationSpeed,
      LayerIndex = layerIndex
    };

    IsHandled = true;
  }

  public static PlayerStateUpdateResult CreateHandled(
    string animationName,
    int animationWeight = 0,
    float animationSpeed = 1f,
    string[] linkedAnimationNames = null,
    int layerIndex = 0)
  {
    return new PlayerStateUpdateResult(
      Animator.StringToHash(animationName),
      animationWeight,
      animationSpeed,
      linkedAnimationNames == null
        ? null
        : linkedAnimationNames.Select(name => Animator.StringToHash(name)).ToArray(),
      layerIndex);
  }

  public static PlayerStateUpdateResult CreateHandled(
    int animationHash,
    int animationWeight = 0,
    float animationSpeed = 1f,
    int[] linkedShortNameHashes = null,
    int layerIndex = 0)
  {
    return new PlayerStateUpdateResult(
      animationHash,
      animationWeight,
      animationSpeed,
      linkedShortNameHashes,
      layerIndex);
  }

  public static PlayerStateUpdateResult Max(
    PlayerStateUpdateResult a,
    PlayerStateUpdateResult b,
    params PlayerStateUpdateResult[] others)
  {
    var max = a.CompareTo(b) > 0 ? a : b;

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

  public static PlayerStateUpdateResult Max(params PlayerStateUpdateResult[] results)
  {
    var max = results.First();

    for (var i = 1; i < results.Length; i++)
    {
      if (results[i].CompareTo(max) > 0)
      {
        max = results[i];
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