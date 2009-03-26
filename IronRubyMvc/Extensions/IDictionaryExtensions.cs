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
                dictionary.ForEach((key, value) => rvd.Add(key.ToString(), (value ?? string.Empty).ToString()));

            return rvd;
        }

        public static ViewDataDictionary ToViewDataDictionary(this IDictionary dictionary)
        {
            var vdd = new ViewDataDictionary();

            if (dictionary != null)
                dictionary.ForEach((key, value) => vdd.Add(key.ToString(), value));

            return vdd;
        }

        public static IDictionary<string, object> ToDictionary(this IDictionary dictionary)
        {
            var dict = new Dictionary<string, object>();

            dictionary.ForEach((key, value) => dict.Add(key.ToString(), value));

            return dict;
        }

        public static void ForEach(this IDictionary dictionary, Action<object, object> iterator)
        {
            foreach (var key in dictionary.Keys)
            {
                iterator(key, dictionary[key]);
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

        private static IEnumerable<TITarget> ToFilters<TITarget>(this IDictionary<object, object> filterDescriptions, string actionName)
            where TITarget : class
        {
            var filters = new List<TITarget>();
            var key = SymbolTable.StringToId(actionName);
            var hasKey = filterDescriptions.ContainsKey(key);
            if (hasKey) filters.AddRange((filterDescriptions[key] as RubyArray).Cast<TITarget>());
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