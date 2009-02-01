using System;
using System.Collections.Generic;

namespace IronRubyMvc
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action )
        {
            foreach (var t in collection)
            {
                action(t);
            }
        }
    }
}