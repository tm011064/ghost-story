using System;
using System.Collections.Generic;

public static class EnumerableExtensions
{
  public static void ForEach<T>(this IEnumerable<T> records, Action<T> action)
  {
    foreach (T item in records)
    {
      action(item);
    }
  }
}
