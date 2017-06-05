using System;
using System.Collections.Generic;

public interface ISpawnable : IFreezable
{
  bool CanSpawn();

  void Reset(IDictionary<string, string> options);

  event EventHandler<GameObjectEventArgs> GotDisabled;
}
