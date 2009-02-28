#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var t in collection)
            {
                action(t);
            }
        }

        public static void ForEach(this IEnumerable collection, Action<object> action)
        {
            foreach (var o in collection)
            {
                action(o);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            foreach (var o in collection)
            {
                return false;
            }
            return true;
        }

        public static bool IsEmpty(this IEnumerable collection)
        {
            foreach (var o in collection)
            {
                return false;
            }
            return true;
        }

        public static bool Contains<T>(this IEnumerable<T> collection, T value) 
        {
            foreach (var t in collection)
            {
                if(t.Equals(value)) return true;
            }
            return false;
        }

        public static bool DoesNotContain<T>(this IEnumerable<T> collection, T value)
        {
            foreach (var t in collection)
            {
                if (!t.Equals(value)) return false;
            }
            return true;
        }

        public static bool DoesNotContain(this IEnumerable collection, object value)
        {
            foreach (var t in collection)
            {
                if (t == value) return false;
            }
            return true;
        }

        public static bool Contains(this IEnumerable collection, object value) 
        {
            foreach (var t in collection)
            {
                if (t.Equals(value)) return true;
            }
            return false;
        }
        
        public static IEnumerable<TSource> Select<TSource>(this IEnumerable<TSource> collection, Func<TSource, bool> predicate)
        {
            foreach (var source in collection)
            {
                if(predicate(source)) yield return source;
            }
        }

        public static IEnumerable Select(this IEnumerable collection, Func<object, bool> predicate)
        {
            foreach (var source in collection)
            {
                if (predicate(source)) yield return source;
            }
        }

       
    }
}