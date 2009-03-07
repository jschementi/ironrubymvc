#region Usings

using System.Collections;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Routing;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;
using Xunit;

#endregion

namespace System.Web.Mvc.IronRuby.Tests.Core
{
    [Concern(typeof (RubyRoutes))]
    public class when_a_value_is_mapped : InstanceContextSpecification<RubyRoutes>
    {
        private RouteCollection _routeCollection;

        protected override void EstablishContext()
        {
            var routes = new RouteCollection();
            routes.Add("my_controller",
                       new Route("my_controller", new MvcRouteHandler())
                           {Constraints = new Hashtable().ToRouteDictionary()});
            _routeCollection = routes;
        }

        protected override RubyRoutes CreateSut()
        {
            return new RubyRoutes(new RouteCollection());
        }

        protected override void Because()
        {
            Sut.MapRoute("my_controller", "my_controller");
        }

        [Observation]
        public void then_it_should_have_a_mapping()
        {
            var obj = (Route) _routeCollection["my_controller"];
            ((Route) Sut["my_controller"]).Url.ShouldBeEqualTo(obj.Url);
        }
    }

    [Concern(typeof (RubyRoutes))]
    public class when_more_values_are_mapped : InstanceContextSpecification<RubyRoutes>
    {
        private RouteCollection _routeCollection;

        protected override void EstablishContext()
        {
            var routes = new RouteCollection
                             {
                                 {
                                     "my_controller", new Route("my_controller", new MvcRouteHandler())
                                                          {Constraints = new Hashtable().ToRouteDictionary()}
                                     }
                             };
            _routeCollection = routes;
        }

        protected override RubyRoutes CreateSut()
        {
            return new RubyRoutes(new RouteCollection());
        }

        protected override void Because()
        {
            Sut.MapRoute("my_controller", "my_controller");
            Sut.MapRoute("my_controller2", "my_controller3");
            Sut.MapRoute("my_controller3", "my_controller2");
        }

        [Observation]
        public void then_it_should_have_a_mapping()
        {
            var obj = (Route) _routeCollection["my_controller"];
            ((Route) Sut["my_controller"]).Url.ShouldBeEqualTo(obj.Url);
        }
    }
}