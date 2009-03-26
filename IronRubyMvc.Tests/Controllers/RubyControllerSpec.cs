#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Routing;
using IronRuby.Builtins;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Extensions;
using Microsoft.Scripting;
using Moq.Mvc;
using Xunit;
using System;

#endregion

namespace System.Web.Mvc.IronRuby.Tests.Controllers
{
    [Concern(typeof (RubyController))]
    public class when_a_controller_is_initialized : with_ironruby_and_an_engine_initialized<RubyController>
    {
        private const string _controllerName = "SomeController";
        protected HttpContextMock _httpContextMock;
        private RequestContext _requestContext;
        private RubyClass _rubyClass;

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
            var controller = _rubyEngine.CreateInstance<RubyController>(_rubyClass);
            controller.InternalInitialize(new ControllerConfiguration { Context = _requestContext, Engine = _rubyEngine, RubyClass = _rubyClass });

            controller.ViewData().Add("test","testing");
//                .ViewData.Add("test", "my value");
            return controller;
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

    [Concern(typeof (RubyController))]
    public class when_asked_to_redirect_to_route : when_a_controller_is_initialized
    {
        private RedirectToRouteResult _result;

        protected override void Because()
        {
            _result = Sut.RedirectToRoute(new Hash(new Dictionary<object, object> {{"MyRoute", "RouteValue"}})) as RedirectToRouteResult;
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_return_a_redirect_to_route_result()
        {
            _result.ShouldBeAnInstanceOf<RedirectToRouteResult>();
        }

        [Observation]
        public void should_return_the_correct_redirect_to_route_result()
        {
            _result.RouteValues["MyRoute"].ShouldNotBeNull();
            _result.RouteValues["MyRoute"].ShouldBeEqualTo("RouteValue");
        }
    }

    [Concern(typeof(RubyController))]
    public class when_asked_to_redirect_to_action : when_a_controller_is_initialized
    {
        private RedirectToRouteResult _result;

        protected override void Because()
        {
            _result = Sut.RedirectToAction("my_action", new Hash(new Dictionary<object, object> { { "MyRoute", "RouteValue" } })) as RedirectToRouteResult;
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_return_a_redirect_to_action_result()
        {
            _result.ShouldBeAnInstanceOf<RedirectToRouteResult>();
        }

        [Observation]
        public void should_return_the_correct_redirect_to_route_result()
        {
            _result.RouteValues["MyRoute"].ShouldNotBeNull();
            _result.RouteValues["MyRoute"].ShouldBeEqualTo("RouteValue");
        }

        [Observation]
        public void should_have_the_correct_action_name()
        {
            _result.RouteValues["action"].ShouldBeEqualTo("my_action");
        }
    }

    [Concern(typeof(RubyController))]
    public class when_asked_for_viewdata : when_a_controller_is_initialized
    {
        private IDictionary _result;

        protected override void Because()
        {
            _result = Sut.ViewData();
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_return_a_redirect_to_action_result()
        {
            _result.ShouldBeAnInstanceOf<Dictionary<object, object>>();
        }
        
    }

    [Concern(typeof(RubyController))]
    public class when_asked_to_execute_an_action : when_a_controller_is_initialized
    {
        private Action _action;

        protected override void Because()
        {
            _action = () => ((IController)Sut).Execute(new RequestContext(_httpContextMock.Object, new RouteData{Values = {{"action", "my_action"}}}));
        }

        [Observation]
        public void should_not_throw()
        {
            _action.ShouldNotThrowAnyExceptions();
        }

        

    }

    [Concern(typeof(RubyController))]
    public class when_asked_for_a_view : when_a_controller_is_initialized
    {
        private ViewResult _result;

        protected override void Because()
        {
            _result = Sut.View();
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_view_data()
        {
            _result.ViewData.ShouldNotBeEmpty();
        }

        [Observation]
        public void should_have_temp_data()
        {
            _result.TempData.ShouldBeEqualTo(Sut.TempData);
        }
    }

    [Concern(typeof(RubyController))]
    public class when_asked_for_a_view_by_name : when_a_controller_is_initialized
    {
        private ViewResult _result;

        protected override void Because()
        {
            _result = Sut.View("my_action");
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_view_data()
        {
            _result.ViewData.ShouldNotBeEmpty();
        }

        [Observation]
        public void should_have_temp_data()
        {
            _result.TempData.ShouldBeEqualTo(Sut.TempData);
        }

        [Observation]
        public void should_have_a_view_name()
        {
            _result.ViewName = "my_action";
        }

    }

    [Concern(typeof(RubyController))]
    public class when_asked_for_a_view_with_an_object : when_a_controller_is_initialized
    {
        private ViewResult _result;
        private object _viewItem;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _viewItem = new object();
        }

        protected override void Because()
        {
            _result = Sut.View(_viewItem);
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_view_data()
        {
            _result.ViewData.Model.ShouldBeEqualTo(_viewItem);
        }

        [Observation]
        public void should_have_temp_data()
        {
            _result.TempData.ShouldBeEqualTo(Sut.TempData);
        }
    }

    [Concern(typeof(RubyController))]
    public class when_asked_for_a_view_with_name_and_master : when_a_controller_is_initialized
    {
        private ViewResult _result;

        protected override void Because()
        {
            _result = Sut.View("my_action", "the_master");
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_view_data()
        {
            _result.ViewData.ShouldNotBeEmpty();
        }

        [Observation]
        public void should_have_temp_data()
        {
            _result.TempData.ShouldBeEqualTo(Sut.TempData);
        }

        [Observation]
        public void should_have_a_view_name()
        {
            _result.ViewName.ShouldBeEqualTo("my_action");
        }

        [Observation]
        public void should_have_a_layout_name()
        {
            _result.MasterName.ShouldBeEqualTo("the_master");
        }
    }

    [Concern(typeof(RubyController))]
    public class when_asked_for_a_view_with_name_and_object : when_a_controller_is_initialized
    {
        private ViewResult _result;
        private object _viewItem;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _viewItem = new object();
        }

        protected override void Because()
        {
            _result = Sut.View("my_action", _viewItem);
        }

        [Observation]
        public void should_return_a_result()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_view_data()
        {
            _result.ViewData.Model.ShouldBeEqualTo(_viewItem);
        }

        [Observation]
        public void should_have_temp_data()
        {
            _result.TempData.ShouldBeEqualTo(Sut.TempData);
        }

        [Observation]
        public void should_have_a_view_name()
        {
            _result.ViewName.ShouldBeEqualTo("my_action");
        }

        
    }
}