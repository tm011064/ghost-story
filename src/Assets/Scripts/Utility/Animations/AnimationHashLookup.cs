using System.Collections.Generic;
using UnityEngine;

public static class AnimationHashLookup
{
  private static readonly Dictionary<string, int> _hashesByName = new Dictionary<string, int>();

  private static readonly Dictionary<int, string> _namesByHash = new Dictionary<int, string>();

  public static void Register(params string[] animationNames)
  {
    foreach (var name in animationNames)
    {
      var hash = Animator.StringToHash(name);

      _hashesByName[name] = hash;
      _namesByHash[hash] = name;
    }
  }

  public static string GetName(int shortNameHash)
  {
    try
    {
      return _namesByHash[shortNameHash];
    }
    catch (KeyNotFoundException)
    {
      Logger.Error("You have to register short name hashes before accessing them. Use the Register method");
      throw;
    }
  }
}
