using System.Collections.Generic;

public class AnimationClipInfo
{
  public int ShortNameHash;

  public HashSet<int> LinkedShortNameHashes;

  public int Weight;

  public float Speed = 1f;

  public int LayerIndex;
}