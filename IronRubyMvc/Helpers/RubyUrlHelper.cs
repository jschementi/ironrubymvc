#region Usings

using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;

#endregion

namespace IronRubyMvcLibrary
{
    public class RubyUrlHelper : UrlHelper
    {
        public RubyUrlHelper(RequestContext viewContext)
            : base(viewContext)
        {
        }

        public new string Action(string actionName)
        {
            return base.Action(actionName);
        }

        public string Action(string actionName, Hash values)
        {
            return base.Action(actionName, values.ToRouteDictionary());
        }

        public new string Action(string actionName, string controllerName)
        {
            return base.Action(actionName, controllerName);
        }

        public string Action(string actionName, string controllerName, Hash values)
        {
            return base.Action(actionName, controllerName, values.ToRouteDictionary());
        }

        public string Action(Hash values)
        {
            return base.RouteUrl(values.ToRouteDictionary());
        }

        public string e(object s)
        {
            return base.Encode(s.ToString());
        }

        public string e(string s)
        {
            return base.Encode(s);
        }
    }
}