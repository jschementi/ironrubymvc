#region Usings

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Tests.Controllers;
using System.Web.Mvc.IronRuby.ViewEngine;
using System.Web.Routing;
using IronRuby.Builtins;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Mvc.IronRuby.ViewEngine;
using Moq;
using Xunit;

#endregion

namespace System.Web.Mvc.IronRuby.Tests.Core
{
    [Concern(typeof (RubyEngine))]
    public class when_asked_for_controller_class_name : StaticContextSpecification
    {
        private string _controllerName;


        protected override void Because()
        {
            _controllerName = RubyEngine.GetControllerClassName("Ninjas");
        }

        [Observation]
        public void it_should_be_the_correct_class_name()
        {
            _controllerName.ShouldBeEqualTo("NinjasController");
        }
    }

    [Concern(typeof(RubyEngine))]
    public class when_asked_to_initialize_ironruby_mvc_with_existing_routes_file : StaticContextSpecification
    {
        private RubyEngine _engine;
        private IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            _pathProvider = Dependency<IPathProvider>();
            _pathProvider.WhenToldTo(pp => pp.ApplicationPhysicalPath).Return(Environment.CurrentDirectory);
            _pathProvider.WhenToldTo(pp => pp.FileExists("~/routes.rb")).Return(true);
            _pathProvider.WhenToldTo(pp => pp.Open("~/routes.rb")).Return(new MemoryStream(new byte[0]));
//            SetupResult.For(RouteTable.Routes).Return(new RouteCollection());
        }

        protected override void Because()
        {
            _engine = RubyEngine.InitializeIronRubyMvc(_pathProvider, "~/routes.rb");
        }
        

        [Observation]
        public void it_should_have_a_global_routes_variable()
        {
            _engine.Context.GetGlobalVariable("routes").ShouldNotBeNull();
        }

        [Observation]
        public void routes_should_be_a_ruby_route_collection()
        {
            _engine.Context.GetGlobalVariable("routes").ShouldBeAnInstanceOf<RubyRoutes>();
        }

        [Observation]
        public void should_have_a_ruby_controller_factory()
        {
            ControllerBuilder.Current.GetControllerFactory().ShouldBeAnInstanceOf<RubyControllerFactory>();
        }

        [Observation]
        public void should_have_a_ruby_view_engine()
        {
            var passes = false;
            ViewEngines.Engines.ForEach(eng =>
                                            {
                                                if(eng is RubyViewEngine)
                                                    passes = true;
                                            });
            passes.ShouldBeTrue();
        }
    }

    [Concern(typeof(RubyEngine))]
    public class when_asked_to_initialize_ironruby_mvc_with_non_existing_routes_file : StaticContextSpecification
    {
        private RubyEngine _engine;
        private IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            _pathProvider = Dependency<IPathProvider>();
            _pathProvider.WhenToldTo(pp => pp.ApplicationPhysicalPath).Return(Environment.CurrentDirectory);
            _pathProvider.WhenToldTo(pp => pp.FileExists("~/routes.rb")).Return(false);
        }

        protected override void Because()
        {
            _engine = RubyEngine.InitializeIronRubyMvc(_pathProvider, "~/routes.rb");
        }


        [Observation]
        public void it_should_not_have_a_global_routes_variable()
        {
            _engine.Context.GetGlobalVariable("routes").ShouldBeNull();
        }


    }

    public abstract class with_an_engine_initialized : with_ironruby_initialized<RubyEngine>
    {
        protected IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _pathProvider = Dependency<IPathProvider>();
            _pathProvider.WhenToldTo(pp => pp.ApplicationPhysicalPath).Return(Environment.CurrentDirectory);
        }

        protected override RubyEngine CreateSut()
        {
            return new RubyEngine(_scriptRuntime, _pathProvider);
        }

        protected void AddClass(string className)
        {
            var script =
                "class {0}; def my_method; $text_var = \"{0}\"; end; def another_method; $text_var = 'from other method'; end; end "
                    .FormattedWith(className);
            _context.DefineGlobalVariable("text_var", "String value");

            Sut.ExecuteScript(script);
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_for_an_existing_ruby_class : with_an_engine_initialized
    {
        private RubyClass _class;


        protected override void Because()
        {
            AddClass("ControllerTestBase");
            _class = Sut.GetRubyClass("ControllerTestBase");
        }

        [Observation]
        public void it_should_be_able_to_get_the_ruby_class()
        {
            _class.Name.ShouldBeEqualTo("ControllerTestBase");
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_to_execute_script : with_an_engine_initialized
    {
        private string _result;


        protected override void Because()
        {
            _result = Sut.ExecuteScript("\"It works from script\".to_clr_string").ToString();
        }

        [Observation]
        public void it_should_be_able_to_get_the_ruby_class()
        {
            _result.ShouldBeEqualTo("It works from script");
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_to_configure_a_controller : with_an_engine_initialized
    {
        private IController _controller;
        private RequestContext _requestContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
        }

        protected override void Because()
        {
            AddController("MyTest");
            _controller = Sut.ConfigureController(Sut.GetRubyClass("MyTestController"), _requestContext);
        }

        [Observation]
        public void it_should_have_a_controller_instance()
        {
            _controller.ShouldNotBeNull();
        }

        [Observation]
        public void it_should_be_a_ruby_controller()
        {
            _controller.ShouldBeAnInstanceOf<RubyController>();
        }

        [Observation]
        public void it_should_have_the_correct_name()
        {
            ((RubyController) _controller).ControllerName.ShouldBeEqualTo("MyTest");
        }

        [Observation]
        public void should_have_the_correct_class_name()
        {
            ((RubyController) _controller).ControllerClassName.ShouldBeEqualTo("MyTestController");
        }

        private void AddController(string controllerName)
        {
            var script =
                "class {0}Controller < Controller; def my_action; $counter = $counter + 1; end; end;".FormattedWith(
                    controllerName);
            Sut.ExecuteScript(script);
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_to_call_a_method : with_an_engine_initialized
    {
        private RubyController _controller;
        private RequestContext _requestContext;
        private string _result;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
        }

        protected override void Because()
        {
            AddController("MyTesting");
            _controller = Sut.ConfigureController(Sut.GetRubyClass("MyTestingController"), _requestContext);
            _result = Sut.CallMethod(_controller, "my_action").ToString();
        }

        [Observation]
        public void it_should_get_the_correct_result()
        {
            _result.ShouldBeEqualTo("Can't see ninjas");
        }

        [Observation]
        public void should_have_the_correct_count()
        {
            _context.GetGlobalVariable("counter").ShouldBeEqualTo(12);
        }

        private void AddController(string controllerName)
        {
            var script = new StringBuilder();
            script.AppendLine("class {0}Controller < Controller".FormattedWith(controllerName));
            script.AppendLine("  def my_action");
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");

            _context.DefineGlobalVariable("counter", 7);

            Sut.ExecuteScript(script.ToString());
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_if_a_underscored_controller_action_exists : with_an_engine_initialized
    {
        private RubyController _controller;
        private RequestContext _requestContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
        }

        protected override void Because()
        {
            AddController("MyTesting");
            _controller = Sut.ConfigureController(Sut.GetRubyClass("MyTestingController"), _requestContext);
        }

        [Observation]
        public void it_should_return_true_for_existing_underscored_message()
        {
            Sut.HasControllerAction(_controller, "my_action").ShouldBeTrue();
        }

        [Observation]
        public void it_should_return_false_for_non_existing_underscored_message()
        {
            Sut.HasControllerAction(_controller, "some_action").ShouldBeFalse();
        }

        [Observation]
        public void it_should_return_true_for_existing_pascal_cased_message()
        {
            Sut.HasControllerAction(_controller, "MyAction").ShouldBeTrue();
        }

        [Observation]
        public void it_should_return_false_for_non_existing_pascal_cased_message()
        {
            Sut.HasControllerAction(_controller, "SomeAction").ShouldBeFalse();
        }

        private void AddController(string controllerName)
        {
            var script = new StringBuilder();
            script.AppendLine("class {0}Controller < Controller".FormattedWith(controllerName));
            script.AppendLine("  def my_action");
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");

            _context.DefineGlobalVariable("counter", 7);

            Sut.ExecuteScript(script.ToString());
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_if_a_pascal_cased_controller_action_exists : with_an_engine_initialized
    {
        private RubyController _controller;
        private RequestContext _requestContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
        }

        protected override void Because()
        {
            AddController("MyTesting");
            _controller = Sut.ConfigureController(Sut.GetRubyClass("MyTestingController"), _requestContext);
        }

        [Observation]
        public void it_should_return_true_for_existing_underscored_message()
        {
            Sut.HasControllerAction(_controller, "internal_initialize").ShouldBeTrue();
        }

        [Observation]
        public void it_should_return_false_for_non_existing_underscored_message()
        {
            Sut.HasControllerAction(_controller, "internal_initialize2").ShouldBeFalse();
        }

        [Observation]
        public void it_should_return_true_for_existing_pascal_cased_message()
        {
            Sut.HasControllerAction(_controller, "InternalInitialize").ShouldBeTrue();
        }

        [Observation]
        public void it_should_return_false_for_non_existing_pascal_cased_message()
        {
            Sut.HasControllerAction(_controller, "InternalInitialize2").ShouldBeFalse();
        }

        private void AddController(string controllerName)
        {
            var script = new StringBuilder();
            script.AppendLine("class {0}Controller < Controller".FormattedWith(controllerName));
            script.AppendLine("  def my_action");
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");

            _context.DefineGlobalVariable("counter", 7);

            Sut.ExecuteScript(script.ToString());
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_to_load_a_controller_with_pascal_cased_name : with_an_engine_initialized
    {
        private RubyController _controller;
        private RequestContext _requestContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            var script = new StringBuilder();
            script.AppendLine("class ATestController < Controller");
            script.AppendLine("  def my_action");
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");
            
            var filePath = "~\\Controllers\\ATestController.rb";
            
            _pathProvider.WhenToldTo(pp => pp.FileExists(filePath)).Return(true);
            _pathProvider.WhenToldTo(pp => pp.Open(filePath)).Return(new MemoryStream(Encoding.UTF8.GetBytes(script.ToString())));

            _requestContext = new RequestContext(Dependency<HttpContextBase>(), new RouteData());
        }

        protected override void Because()
        {
            _controller = Sut.LoadController(_requestContext, "ATest");
        }

        [Observation]
        public void it_should_have_a_controller_instance()
        {
            _controller.ShouldNotBeNull();
        }

        [Observation]
        public void it_should_be_a_ruby_controller()
        {
            _controller.ShouldBeAnInstanceOf<RubyController>();
        }

        [Observation]
        public void it_should_have_the_correct_name()
        {
            _controller.ControllerName.ShouldBeEqualTo("ATest");
        }

        [Observation]
        public void should_have_the_correct_class_name()
        {
            _controller.ControllerClassName.ShouldBeEqualTo("ATestController");
        }
    }

    [Concern(typeof(RubyEngine))]
    public class when_asked_to_load_a_non_existing_controller : with_an_engine_initialized
    {
        private RubyController _controller;
        private RequestContext _requestContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _pathProvider.WhenToldTo(pp => pp.FileExists(null)).IgnoreArguments().Return(false);

            _requestContext = new RequestContext(Dependency<HttpContextBase>(), new RouteData());
        }

        protected override void Because()
        {
            _controller = Sut.LoadController(_requestContext, "ATestd");
        }

        [Observation]
        public void it_should_not_have_a_controller_instance()
        {
            _controller.ShouldBeNull();
        }


    }


    [Concern(typeof(RubyEngine))]
    public class when_asked_to_load_a_controller_with_underscored_name : with_an_engine_initialized
    {
        private RubyController _controller;
        private RequestContext _requestContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            var script = new StringBuilder();
            script.AppendLine("class ATestController < Controller");
            script.AppendLine("  def my_action");
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");

            var filePath = "~\\Controllers\\a_test_controller.rb";

            _pathProvider.WhenToldTo(pp => pp.FileExists(filePath)).Return(true);
            _pathProvider.WhenToldTo(pp => pp.Open(filePath)).Return(new MemoryStream(Encoding.UTF8.GetBytes(script.ToString())));

            _requestContext = new RequestContext(Dependency<HttpContextBase>(), new RouteData());
        }

        protected override void Because()
        {
            _controller = Sut.LoadController(_requestContext, "ATest");
        }

        [Observation]
        public void it_should_have_a_controller_instance()
        {
            _controller.ShouldNotBeNull();
        }

        [Observation]
        public void it_should_be_a_ruby_controller()
        {
            _controller.ShouldBeAnInstanceOf<RubyController>();
        }

        [Observation]
        public void it_should_have_the_correct_name()
        {
            _controller.ControllerName.ShouldBeEqualTo("ATest");
        }

        [Observation]
        public void should_have_the_correct_class_name()
        {
            _controller.ControllerClassName.ShouldBeEqualTo("ATestController");
        }
    }
}