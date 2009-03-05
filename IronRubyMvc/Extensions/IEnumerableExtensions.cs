#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Helpers;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "o")]
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            foreach (var o in collection)
            {
                return false;
            }
            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "o")]
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
                if((t.IsNull() && value.IsNull()) || (t.IsNotNull() && t.Equals(value))) return true;
            }
            return false;
        }

        public static bool DoesNotContain<T>(this IEnumerable<T> collection, T value)
        {
            return !collection.Contains(value);
        }

        public static bool DoesNotContain(this IEnumerable collection, object value)
        {
            return !collection.Contains(value);
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

        internal static FilterInfo ToFilterInfo(this IEnumerable filterDescriptions, string actionName, IRubyEngine engine)
        {
            var filterInfo = new FilterInfo();

            filterDescriptions.ToActionFilters(actionName, engine).ForEach(filter => filterInfo.ActionFilters.Add(filter));
            filterDescriptions.ToAuthorizationFilters(actionName, engine).ForEach(filter => filterInfo.AuthorizationFilters.Add(filter));
            filterDescriptions.ToExceptionFilters(actionName, engine).ForEach(filter => filterInfo.ExceptionFilters.Add(filter));
            filterDescriptions.ToResultFilters(actionName, engine).ForEach(filter => filterInfo.ResultFilters.Add(filter));

            return filterInfo;
        }

        private static IEnumerable<TITarget> ToFilters<TITarget, TConverter>(this IEnumerable filterDescriptions, string actionName, IRubyEngine engine)
            where TITarget : class
            where TConverter : HashConverter<TITarget>, new()
        {
            var filters = new List<TITarget>();
            var converter = new TConverter();

            filterDescriptions.ForEach(filterDescription =>
            {
                var description = filterDescription as Hash;
                if (description.IsNull()) return;

                var key = description[SymbolConstants.Name].ToString();
                if (new string[] { "controller", null, string.Empty, actionName }.DoesNotContain(key)) return; 

                var filter = converter.Convert(filterDescription as Hash, engine);
                if (filter.IsNotNull())
                    filters.Add(filter);

            });
            return filters;
        }

        private static IEnumerable<IAuthorizationFilter> ToAuthorizationFilters(this IEnumerable filterDescriptions, string actionName, IRubyEngine engine)
        {
            return filterDescriptions.ToFilters<IAuthorizationFilter, HashToAuthorizationFilterConverter>(actionName, engine);
        }

        private static IEnumerable<IExceptionFilter> ToExceptionFilters(this IEnumerable filterDescriptions, string actionName, IRubyEngine engine)
        {
            return filterDescriptions.ToFilters<IExceptionFilter, HashToExceptionFilterConverter>(actionName, engine);
        }

        private static IEnumerable<IActionFilter> ToActionFilters(this IEnumerable filterDescriptions, string actionName, IRubyEngine engine)
        {
            return filterDescriptions.ToFilters<IActionFilter, HashToActionFilterConverter>(actionName, engine);
        }

        private static IEnumerable<IResultFilter> ToResultFilters(this IEnumerable filterDescriptions, string actionName, IRubyEngine engine)
        {
            return filterDescriptions.ToFilters<IResultFilter, HashToResultFilterConverter>(actionName, engine);
        }

        internal static IEnumerable<ActionSelector> ToActionSelectors(this IEnumerable selectorDescriptions)
        {
            var selectors = new List<ActionSelector>();

            selectorDescriptions.ForEach(selector => selectors.Add(c => true));

            return selectors;
        }

        
    }
}