using System;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;
using System.Collections;
using System.Collections.Generic;

namespace BugWorkarounds
{
    public static class Workarounds
    {
        // IronRuby currently is a little bit confused about Actions and extension methods when called with nil/null
        // These methods get around those confusions
        public static bool IsNull(object value) { return value.IsNull(); }
        public static bool IsNotNull(object value) { return value.IsNotNull(); }
        public static bool IsNullOrBlank(string value) { return value.IsNullOrBlank(); }
        public static bool IsNotNullOrBlank(string value) { return value.IsNotNullOrBlank(); }
        public static bool IsEmpty(IEnumerable collection) { return collection.IsEmpty(); }
        public static bool IsEmpty<T>(IEnumerable<T> collection) { return collection.IsEmpty(); }
        public static Action<object> WrapProc(Proc proc) { return obj => proc.Call(obj); }
        public static Action<T> WrapProc<T>(Proc proc) { return obj => proc.Call(obj); }
    }

}