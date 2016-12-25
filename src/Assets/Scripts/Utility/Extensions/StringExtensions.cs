using System;
using System.Linq;

public static class StringExtensions
{
  public static T ToEnum<T>(this string value) where T : struct
  {
    return (T)Enum.Parse(typeof(T), value);
  }

  public static T[] ToManyEnums<T>(this string value) where T : struct
  {
    return value
      .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
      .Select(s => s.Trim().ToEnum<T>())
      .ToArray();
  }
}
