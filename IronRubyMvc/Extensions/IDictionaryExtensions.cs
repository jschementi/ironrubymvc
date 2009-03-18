#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Web.Routing;
using IronRuby.Builtins;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Extensions
{
    public static class IDictionaryExtensions
    {
        public static RouteValueDictionary ToRouteDictionary(this IDictionary dictionary)
        {
            var rvd = new RouteValueDictionary();

            if (dictionary.IsNotNull())
                dictionary.Keys.ForEach(key => rvd.Add(key.ToString(), (dictionary[key] ?? string.Empty).ToString()));

            return rvd;
        }

        public static ViewDataDictionary ToViewDataDictionary(this IDictionary dictionary)
        {
            var vdd = new ViewDataDictionary();

            if (dictionary != null)
                foreach (var key in dictionary.Keys)
                    vdd.Add(key.ToString(), dictionary[key]);

            return vdd;
        }

        public static void ForEach(this IDictionary dictionary, Action<object, object> iterator)
        {
            foreach (var key in dictionary.Keys)
            {
                iterator(key, dictionary[key]);
            }
        }

        private static IEnumerable<TITarget> ToFilters<TITarget>(this IDictionary<object, object> filterDescriptions, string actionName)
            where TITarget : class
        {
            var filters = new List<TITarget>();
            var key = SymbolTable.StringToId(actionName);
            var hasKey = filterDescriptions.ContainsKey(key);
            if(hasKey) filters.AddRange((filterDescriptions[key] as RubyArray).Cast<TITarget>());
            return filters;
        }

        internal static IEnumerable<IAuthorizationFilter> ToAuthorizationFilters(this IDictionary<object, object> filterDescriptions, string actionName)
        {
            return filterDescriptions.ToFilters<IAuthorizationFilter>(actionName);
        }

        internal static IEnumerable<IExceptionFilter> ToExceptionFilters(this IDictionary<object, object> filterDescriptions, string actionName)
        {
            return filterDescriptions.ToFilters<IExceptionFilter>(actionName);
        }

        internal static IEnumerable<IActionFilter> ToActionFilters(this IDictionary<object, object> filterDescriptions, string actionName)
        {
            return filterDescriptions.ToFilters<IActionFilter>(actionName);
        }

        internal static IEnumerable<IResultFilter> ToResultFilters(this IDictionary<object, object> filterDescriptions, string actionName)
        {
            return filterDescriptions.ToFilters<IResultFilter>(actionName);
        }

        
    }
}