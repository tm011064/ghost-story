using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Editor.Tiled.Xml
{
  public static class PolyLineExtensions
  {
    public static IEnumerable<Vector2> ToVectors(this PolyLine polyLine)
    {
      return polyLine
        .Points
        .Split(' ')
        .Select(
          c =>
          {
            var points = c.Split(',');
            return new Vector2(int.Parse(points.First()), -int.Parse(points.Last()));
          });
    }
  }
}
