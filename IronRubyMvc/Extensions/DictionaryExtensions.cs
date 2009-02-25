#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;

#endregion

namespace IronRubyMvcLibrary.Extensions
{
    public static class DictionaryExtensions
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

        public static IEnumerable<ActionSelector> ToActionSelectors(this IDictionary dictionary)
        {
            var selectors = new List<ActionSelector>(dictionary.Keys.Count);

            dictionary.ForEach((key, value) => selectors.Add(c => true));

            return selectors.ToArray();
        }

        public static IEnumerable<RubyAuthorizationFilter> ToAuthorizationFilters(this IDictionary dictionary)
        {
            var filters = new List<RubyAuthorizationFilter>(dictionary.Keys.Count);
            dictionary.ForEach((key, value) =>
                                   {
                                       var filterDescription = dictionary[key] as Hash;
                                       var authFilter =
                                           new HashToAuthorizationFilterConverter(filterDescription).Convert();
                                       if (authFilter.IsNotNull()) filters.Add(authFilter);
                                   });
            return filters;
        }

        public static IEnumerable<RubyErrorFilter> ToErrorFilters(this IDictionary dictionary)
        {
            var filters = new List<RubyErrorFilter>(dictionary.Keys.Count);
            dictionary.ForEach((key, value) =>
                                   {
                                       var filterDescription = dictionary[key] as Hash;
                                       var authFilter = new HashToErrorFilterConverter(filterDescription).Convert();
                                       if (authFilter.IsNotNull()) filters.Add(authFilter);
                                   });
            return filters;
        }

        public static IEnumerable<RubyActionFilter> ToActionFilters(this IDictionary dictionary)
        {
            var filters = new List<RubyActionFilter>(dictionary.Keys.Count);
            dictionary.ForEach((key, value) =>
                                   {
                                       var filterDescription = dictionary[key] as Hash;
                                       var actionFilter = new HashToActionFilterConverter(filterDescription).Convert();
                                       if (actionFilter.IsNotNull()) filters.Add(actionFilter);
                                   });
            return filters;
        }

        public static void ForEach(this IDictionary dictionary, Action<object, object> iterator)
        {
            foreach (var key in dictionary.Keys)
            {
                iterator(key, dictionary[key]);
            }
        }
    }
}