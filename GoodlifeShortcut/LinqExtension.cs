using System;
using System.Collections.Generic;
using System.Text;

namespace GoodlifeShortcut;

public static class LinqExtension
{
    public static IEnumerable<(int key, T element)> Index<T>(this IEnumerable<T> enumerable)
    {
        int i = 0;
        foreach (var item in enumerable)
            yield return (i++, item);
    }
}
