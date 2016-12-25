using System;
using System.Collections.Generic;

public interface ISpawnable
{
  bool CanSpawn();

  void Reset(IDictionary<string, string> options);

  event Action<BaseMonoBehaviour> GotDisabled;
}