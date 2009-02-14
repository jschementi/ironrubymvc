#region Usings

using System;
using System.Collections;
using System.Collections.Generic;

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
//
//        public static IEnumerable<TTarget> Collect<TSourc, TTarget>(this IEnumerable<TSourc> collection,
//                                                                    Func<TSourc, TTarget> target)
//        {
//            foreach (var source in collection)
//            {
//                yield return target(source);
//            }
//        }
//
//        public static IEnumerable Collect(this IEnumerable collection, Func<object, object> target)
//        {
//            foreach (var o in collection)
//            {
//                yield return target(o);
//            }
//        }
//
//        public static IEnumerable<TTarget> Zip<TTarget>(this IEnumerable<TTarget> collection)
//        {
//            foreach (var target in collection)
//            {
//                if (target.IsNotNull())
//                    yield return target;
//            }
//        }
//
//        public static IEnumerable<TTarget> Where<TSource, TTarget>(this IEnumerable<TSource> collection, Func<TSource, TTarget> target)
//        {
//            foreach (var source in collection)
//            {
//                var tgt = target(source);
//                if(tgt.IsNotNull())
//                    yield return tgt;
//            }
//        }
    }
}