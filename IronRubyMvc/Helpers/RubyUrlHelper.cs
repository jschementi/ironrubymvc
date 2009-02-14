#region Usings

using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Helpers
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
            return Action(actionName, values.ToRouteDictionary());
        }

        public new string Action(string actionName, string controllerName)
        {
            return base.Action(actionName, controllerName);
        }

        public string Action(string actionName, string controllerName, Hash values)
        {
            return Action(actionName, controllerName, values.ToRouteDictionary());
        }

        public string Action(Hash values)
        {
            return RouteUrl(values.ToRouteDictionary());
        }

        public string E(object value)
        {
            return Encode(value.ToString());
        }

        public string E(string value)
        {
            return Encode(value);
        }
    }
}