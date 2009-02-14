using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Xunit;

namespace IronRubyMvcLibrary.Tests.Core
{
    [Concern(typeof (RubyRouteCollection))]
    public class when_a_value_is_mapped : InstanceContextSpecification<RubyRouteCollection>
    {
        private RouteCollection _routeCollection;
        protected override void EstablishContext()
        {
            var routes = new RouteCollection();
            routes.Add("my_controller", new Route("my_controller", new MvcRouteHandler()){Constraints = new Hashtable().ToRouteDictionary()});
            _routeCollection = routes;
        }

        protected override RubyRouteCollection CreateSut()
        {
            return new RubyRouteCollection(new RouteCollection());
        }

        protected override void Because()
        {
            Sut.MapRoute("my_controller", "my_controller");
        }

        [Observation]
        public void then_it_should_have_a_mapping()
        {
            var obj = (Route)_routeCollection["my_controller"];
            ((Route)Sut["my_controller"]).Url.ShouldBeEqualTo(obj.Url);
        }
    }

    [Concern(typeof(RubyRouteCollection))]
    public class when_more_values_are_mapped : InstanceContextSpecification<RubyRouteCollection>
    {
        private RouteCollection _routeCollection;
        protected override void EstablishContext()
        {
            var routes = new RouteCollection();
            routes.Add("my_controller", new Route("my_controller", new MvcRouteHandler()) { Constraints = new Hashtable().ToRouteDictionary() });
            _routeCollection = routes;
        }

        protected override RubyRouteCollection CreateSut()
        {
            return new RubyRouteCollection(new RouteCollection());
        }

        protected override void Because()
        {
            Sut.MapRoute("my_controller", "my_controller", null);
            Sut.MapRoute("my_controller2", "my_controller3", null);
            Sut.MapRoute("my_controller3", "my_controller2", null);
        }

        [Observation]
        public void then_it_should_have_a_mapping()
        {
            var obj = (Route)_routeCollection["my_controller"];
            ((Route)Sut["my_controller"]).Url.ShouldBeEqualTo(obj.Url);
        }
    }
}