#region Usings

using System.Collections;
using System.Web.Mvc;
using System.Web.Routing;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.Core
{
    public class RubyRouteCollection
    {
        private readonly RouteCollection routes;

        public RubyRouteCollection(RouteCollection routes)
        {
            this.routes = routes;
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

        public RouteBase this[string name]
        {
            get { return routes[name]; }
        }
    }
}