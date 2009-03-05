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

        public static void ForEach(this IDictionary dictionary, Action<object, object> iterator)
        {
            foreach (var key in dictionary.Keys)
            {
                iterator(key, dictionary[key]);
            }
        }
    }
}