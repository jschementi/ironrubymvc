#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Helpers;

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

        public static IEnumerable<TITarget> ToFilters<TITarget, TTarget, TConverter>(this IDictionary dictionary, IRubyEngine engine)
            where TITarget : class
            where TTarget : class, TITarget
            where TConverter : HashConverter<TTarget>, new()
        {
            var filters = new List<TITarget>(dictionary.Keys.Count);
            var converter = new TConverter();
            dictionary.ForEach((key, value) =>
                                   {
                                       var filter = dictionary[key];
                                       if (filter.IsNull()) return;
                                       var filterDescription = filter as Hash;
                                       if (filterDescription.IsNotNull())
                                       {
                                           var authFilter = converter.Convert(filterDescription);
                                           if (authFilter.IsNotNull()) filters.Add(authFilter);
                                       }
                                       else if (filter is RubyClass)
                                       {
                                           filters.Add(engine.CreateInstance<TITarget>(filter as RubyClass));
                                       }
                                       else if (filter is TITarget)
                                       {
                                           filters.Add(filter as TITarget);
                                       }
                                   });
            return filters;
        }

        public static IEnumerable<IAuthorizationFilter> ToAuthorizationFilters(this IDictionary dictionary, IRubyEngine engine)
        {
            return
                dictionary.ToFilters
                    <IAuthorizationFilter, RailsStyleAuthorizationFilter, HashToAuthorizationFilterConverter>(engine);
        }

        public static IEnumerable<IExceptionFilter> ToExceptionFilters(this IDictionary dictionary, IRubyEngine engine)
        {
            return
                dictionary.ToFilters<IExceptionFilter, RailsStyleExceptionFilter, HashToExceptionFilterConverter>(engine);
        }

        public static IEnumerable<IActionFilter> ToActionFilters(this IDictionary dictionary, IRubyEngine engine)
        {
            return
                dictionary.ToFilters<IActionFilter, RailsStyleActionFilter, HashToActionFilterConverter>(engine);
        }

        public static IEnumerable<IResultFilter> ToResultFilters(this IDictionary dictionary, IRubyEngine engine)
        {
            return dictionary.ToFilters<IResultFilter, RailsStyleResultFilter, HashToResultFilterConverter>(engine);
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