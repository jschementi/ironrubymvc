#region Usings

using System.Collections;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Routing;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Core
{
    public class RubyRoutes
    {
        private readonly RouteCollection _routes;

        public RubyRoutes(RouteCollection routes)
        {
            _routes = routes;
        }

        public RouteBase this[string name]
        {
            get { return _routes[name]; }
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
            _routes.Add(name, new Route(url, new MvcRouteHandler())
                                  {
                                      Defaults = defaults.ToRouteDictionary(),
                                      Constraints = constraints.ToRouteDictionary()
                                  });
        }

        public void MapRoute(string name, string url, string[] namespaces)
        {
            MapRoute(name, url, new Hashtable(), new Hashtable(), namespaces);
        }

        public void MapRoute(string name, string url, IDictionary defaults, string[] namespaces)
        {
            MapRoute(name, url, defaults, new Hashtable(), namespaces);
        }

        public void MapRoute(string name, string url, IDictionary defaults, IDictionary constraints, string[] namespaces)
        {
            var route = new Route(url, new MvcRouteHandler())
                            {
                                Defaults = defaults.ToRouteDictionary(),
                                Constraints = constraints.ToRouteDictionary(),
                                DataTokens = new RouteValueDictionary()
                            };
            route.DataTokens["Namespaces"] = namespaces;
            _routes.Add(name, route );
        }

//        public void MapRoute(string name, string url, MutableString[] namespaces)
//        {
//            MapRoute(name, url, new Hashtable(), new Hashtable(), namespaces);
//        }
//
//        public void MapRoute(string name, string url, IDictionary defaults, MutableString[] namespaces)
//        {
//            MapRoute(name, url, defaults, new Hashtable(), namespaces);
//        }
//
//        public void MapRoute(string name, string url, IDictionary defaults, IDictionary constraints, MutableString[] namespaces)
//        {
//            var route = new Route(url, new MvcRouteHandler())
//            {
//                Defaults = defaults.ToRouteDictionary(),
//                Constraints = constraints.ToRouteDictionary(),
//                DataTokens = new RouteValueDictionary()
//            };
//            route.DataTokens["Namespaces"] = namespaces.Cast<string>();
//            _routes.Add(name, route);
//        }

        public void IgnoreRoute(string url)
        {
            IgnoreRoute(url, new Hashtable());
        }

        public void IgnoreRoute(string url, IDictionary constraints)
        {
            var route = new IgnoreRouteInternal(url)
            {
                Constraints = constraints.ToRouteDictionary()
            };

            _routes.Add(route);
        }

        private sealed class IgnoreRouteInternal : Route
        {
            public IgnoreRouteInternal(string url)
                : base(url, new StopRoutingHandler())
            {
            }

            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues)
            {
                // Never match during route generation. This avoids the scenario where an IgnoreRoute with
                // fairly relaxed constraints ends up eagerly matching all generated URLs.
                return null;
            }
        }
    }
}