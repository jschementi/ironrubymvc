#region Usings

using System.Collections;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Routing;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyRoutes
    {
        private readonly RouteCollection routes;

        public RubyRoutes(RouteCollection routes)
        {
            this.routes = routes;
        }

        public RouteBase this[string name]
        {
            get { return routes[name]; }
        }

        public void MapRoute(string name, string url)
        {
            MapRoute(name, url, new Hashtable(), new Hashtable());
        }

        public void MapRoute(string name, string url, IDictionary defaults)
        {
            MapRoute(name, url, defaults, new Hashtable());
        }

        public void MapRoute(string name, string url, IDictionary defaults, IDictionary constraints)
        {
            routes.Add(name, new Route(url, new MvcRouteHandler())
                                 {
                                     Defaults = defaults.ToRouteDictionary(),
                                     Constraints = constraints.ToRouteDictionary()
                                 });
        }
    }
}