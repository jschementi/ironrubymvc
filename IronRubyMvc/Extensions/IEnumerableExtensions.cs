#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using IronRuby.Builtins;

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
            // not guarding for null foreach does that for me
            foreach (var o in collection)
            {
                return false;
            }
            return true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "o")]
        public static bool IsEmpty(this IEnumerable collection)
        {
            // not guarding for null, foreach does that for me
            foreach (var o in collection)
            {
                return false;
            }
            return true;
        }

        internal static bool DoesNotContain<TSource>(this IEnumerable<TSource> collection, Func<TSource, bool> predicate)
        {
            foreach (var o in collection)
            {
                if (predicate(o)) return false;
            }
            return true;
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

        internal static IEnumerable<SelectListItem> ToSelectListItemList(this IEnumerable collection)
        {
            var result = new List<SelectListItem>();

            collection.ForEach(item =>
                                   {
                                       var hash = (Hash) item;
                                       var li = new SelectListItem();
                                       hash.ForEach((key, value) =>
                                                        {
                                                            if (key.ToString() == "text")
                                                                li.Text = value.ToString();
                                                            if (key.ToString() == "value")
                                                                li.Value = value.ToString();
                                                            if (key.ToString() == "selected")
                                                                li.Selected = (bool) value;
                                                        });
                                       result.Add(li);
                                   });
            return result;
        }
    }
}