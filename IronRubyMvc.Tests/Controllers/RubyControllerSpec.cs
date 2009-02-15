using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;
using Moq.Mvc;
using Xunit;

namespace IronRubyMvcLibrary.Tests.Controllers
{

    [Concern(typeof (RubyController))]
    public class when_a_controller_is_initialized : with_ironruby_and_an_engine_initialized<RubyController>
    {
        private const string _controllerName = "SomeController";
        private RubyClass _rubyClass;
        private RequestContext _requestContext;
        protected HttpContextMock _httpContextMock;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            var script = new StringBuilder();
            script.AppendLine("class {0} < Controller".FormattedWith(_controllerName));
            script.AppendLine("  def my_action");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");

            _rubyEngine.ExecuteScript(script.ToString());
             _rubyClass = _rubyEngine.GetRubyClass(_controllerName);

            _httpContextMock = new HttpContextMock();
            var httpContext = _httpContextMock.Object;

//            EstablishHttpContext(httpContext);

            _requestContext = new RequestContext(httpContext, new RouteData());
            
        }

//        protected virtual void EstablishHttpContext(HttpContextBase httpContext)
//        {
//            
//        }

        protected override RubyController CreateSut()
        {
            return _rubyEngine.ConfigureController(_rubyClass, _requestContext);


        }

        protected override void Because()
        {

        }

        [Observation]
        public void should_have_the_correct_controller_class_name()
        {
            Sut.ControllerClassName.ShouldBeEqualTo(_controllerName);
        }

        [Observation]
        public void should_have_the_correct_controller_name()
        {
            Sut.ControllerName.ShouldBeEqualTo("Some");
        }
    }

    [Concern(typeof (RubyController))]
    public class when_being_asked_for_the_params : when_a_controller_is_initialized
    {
        private IDictionary<object, object> _params;
        protected override void EstablishContext()
        {
            base.EstablishContext();
            var form = new NameValueCollection
                           {
                               {"a_form_field", "a form value"}
                           };
            var queryString = new NameValueCollection
                                  {
                                      {"a_query_string_field", "a query string value"}
                                  };
            _httpContextMock.HttpRequest.Expect(r => r.Form).Returns(form);
            _httpContextMock.HttpRequest.Expect(r => r.QueryString).Returns(queryString);

        }

        protected override RubyController CreateSut()
        {
            var sut = base.CreateSut();
            var routeData = sut.ControllerContext.RouteData.Values;
            routeData.Clear();
            routeData.Add("route_data_field", "a route data value");

            return sut;
        }
        
        protected override void Because()
        {
           _params = Sut.Params;
        }

        [Observation]
        public void should_have_params()
        {
            _params.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_a_form_field()
        {
            _params[SymbolTable.StringToId("a_form_field")].ShouldNotBeNull();
        }

        [Observation]
        public void should_have_a_form_field_value()
        {
            _params[SymbolTable.StringToId("a_form_field")].ShouldBeEqualTo("a form value");
        }

        [Observation]
        public void should_have_a_query_string_field()
        {
            _params[SymbolTable.StringToId("a_query_string_field")].ShouldNotBeNull();
        }

        [Observation]
        public void should_have_a_query_string_field_value()
        {
            _params[SymbolTable.StringToId("a_query_string_field")].ShouldBeEqualTo("a query string value");
        }

        [Observation]
        public void should_have_a_route_data_field()
        {
            _params[SymbolTable.StringToId("route_data_field")].ShouldNotBeNull();
        }

        [Observation]
        public void should_have_a_route_data_field_value()
        {
            _params[SymbolTable.StringToId("route_data_field")].ShouldBeEqualTo("a route data value");
        }
    }

//    [Concern(typeof (RubyController))]
//    public class when_asked_to_execute : when_a_controller_is_initialized
//    {
//        private Action _action;
//
//        protected override void Because()
//        {
//            _action = () => Sut.
//        }
//
//        [Observation]
//        public void first_observation()
//        {
//
//        }
//    }
}