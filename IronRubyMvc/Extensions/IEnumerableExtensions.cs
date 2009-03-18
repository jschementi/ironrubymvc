#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace System.Web.Mvc.IronRuby.Extensions
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

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "o")]
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            foreach (var o in collection)
            {
                return false;
            }
            return true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "o")]
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
                if ((t.IsNull() && value.IsNull()) || (t.IsNotNull() && t.Equals(value))) return true;
            }
            return false;
        }

        public static bool DoesNotContain<T>(this IEnumerable<T> collection, T value)
        {
            foreach (var t in collection)
            {
                if (t.Equals(value)) return false;
            }
            return true;
        }

        public static bool DoesNotContain(this IEnumerable collection, object value)
        {
            foreach (var t in collection)
            {
                if (t.Equals(value)) return false;
            }
            return true;
        }

        public static bool DoesNotContain<TSource>(this IEnumerable<TSource> collection, Func<TSource, bool> predicate)
        {
            foreach (var o in collection)
            {
                if (predicate(o)) return false;
            }
            return true;
        }

        public static bool Contains(this IEnumerable collection, Func<object, bool> predicate)
        {
            foreach (var o in collection)
            {
                if(predicate(o)) return true;
            }
            return false;
        }

        public static bool Contains(this IEnumerable collection, object value)
        {
            foreach (var t in collection)
            {
                if (t.Equals(value)) return true;
            }
            return false;
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> collection, Func<TSource, bool> predicate)
        {
            foreach (var source in collection)
            {
                if (predicate(source)) yield return source;
            }
        }

        public static bool All<TSource>(this IEnumerable<TSource> collection, Func<TSource, bool> predicate)
        {
            foreach (var source in collection)
            {
                if(!predicate(source)) return false;
            }
            return true;
        }

        public static IEnumerable Where(this IEnumerable collection, Func<object, bool> predicate)
        {
            foreach (var source in collection)
            {
                if (predicate(source)) yield return source;
            }
        }

        internal static FilterInfo ToFilterInfo(this IDictionary<object, object> filterDescriptions, string actionName)
        {
            var filterInfo = new FilterInfo();

            filterDescriptions.ToActionFilters(actionName).ForEach(filter => filterInfo.ActionFilters.Add(filter));
            filterDescriptions.ToAuthorizationFilters(actionName).ForEach(filter => filterInfo.AuthorizationFilters.Add(filter));
            filterDescriptions.ToExceptionFilters(actionName).ForEach(filter => filterInfo.ExceptionFilters.Add(filter));
            filterDescriptions.ToResultFilters(actionName).ForEach(filter => filterInfo.ResultFilters.Add(filter));

            return filterInfo;
        }

        internal static IEnumerable<TTarget> Cast<TTarget>(this IEnumerable collection) where TTarget : class
        {
            var result = new List<TTarget>();
            collection.ForEach(item =>
                                   {
                                       var casted = (typeof(TTarget) == typeof(string)) ? item.ToString() as TTarget : item as TTarget;
                                       if (casted.IsNotNull()) result.Add(casted);
                                   });
            return result;
        }

        internal static IEnumerable<TTarget> Map<TSource, TTarget>(this IEnumerable<TSource> collection, Func<TSource, TTarget> iterator)
        {
            foreach (var source in collection)
            {
                yield return iterator(source);
            }
        }

        internal static IEnumerable Map(this IEnumerable collection, Func<object, object> iterator)
        {
            foreach (var source in collection)
            {
                yield return iterator(source);
            }
        }

        internal static int Count(this IEnumerable collection)
        {
            var count = 0;
            foreach (var o in collection)
            {
                count++;
            }
            return count;
        }

        internal static TSource[] ToArray<TSource>(this IEnumerable<TSource> collection)
        {
            var result = new TSource[collection.Count()];
            var idx = 0;

            collection.ForEach(item => result[idx++] = item);

            return result;
        }

    }
}