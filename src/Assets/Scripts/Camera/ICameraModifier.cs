using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface ICameraModifier
{
  bool Contains(Vector2 point);

  void Activate();
}
