using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Routing;
using IronRuby.Builtins;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Extensions;
using Moq.Mvc;
using Xunit;

namespace System.Web.Mvc.IronRuby.Tests.Controllers
{

    [Concern(typeof (RubyControllerDescriptor))]
    public class when_a_descriptor_executes_for_an_existing_action : with_ironruby_and_an_engine_initialized<RubyControllerDescriptor>
    {
        private const string _controllerName = "WarriorsController";
        private RubyClass _rubyClass;
        private RubyActionDescriptor _result;
        private ControllerContext _controllerContext;

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

            var httpContext = new HttpContextMock().Object;
            var requestContext = new RequestContext(httpContext, new RouteData());
            var controller = _rubyEngine.ConfigureController(_rubyEngine.GetRubyClass(_controllerName),
                                                             requestContext);
            _controllerContext = new ControllerContext(requestContext, controller);
        }

        protected override RubyControllerDescriptor CreateSut()
        {
            return new RubyControllerDescriptor(_rubyClass) { RubyEngine = _rubyEngine };
        }

        protected override void Because()
        {
            _result = Sut.FindAction(_controllerContext, "my_action") as RubyActionDescriptor;
        }

        [Observation]
        public void should_not_be_null()
        {
            _result.ShouldNotBeNull();
        }

        [Observation]
        public void should_be_for_the_correct_method()
        {
            _result.ActionName.ShouldBeEqualTo("my_action");
        }
    }

    [Concern(typeof(RubyControllerDescriptor))]
    public class when_a_descriptor_executes_for_a_non_existing_action : with_ironruby_and_an_engine_initialized<RubyControllerDescriptor>
    {
        private const string _controllerName = "WarriorsController";
        private RubyClass _rubyClass;
        private RubyActionDescriptor _result;
        private ControllerContext _controllerContext;

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

            var httpContext = new HttpContextMock().Object;
            var requestContext = new RequestContext(httpContext, new RouteData());
            var controller = _rubyEngine.ConfigureController(_rubyEngine.GetRubyClass(_controllerName),
                                                             requestContext);
            _controllerContext = new ControllerContext(requestContext, controller);
        }

        protected override RubyControllerDescriptor CreateSut()
        {
            return new RubyControllerDescriptor(_rubyClass) { RubyEngine = _rubyEngine };
        }

        protected override void Because()
        {
            _result = Sut.FindAction(_controllerContext, "my_actionss") as RubyActionDescriptor;
        }

        [Observation]
        public void should_be_null()
        {
            _result.ShouldBeNull();
        }
    }

    [Concern(typeof (RubyControllerDescriptor))]
    public class when_a_descriptor_is_initialized : with_ironruby_and_an_engine_initialized<RubyControllerDescriptor>
    {
        private const string _controllerName = "WarriorsController";
        
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
        }

        protected override RubyControllerDescriptor CreateSut()
        {
            return new RubyControllerDescriptor(_rubyClass) {RubyEngine = _rubyEngine};
        }

        protected override void Because()
        {
            
        }

        [Observation]
        public void should_have_a_controller_name()
        {
            Sut.ControllerName.ShouldNotBeEmpty();
        }

        [Observation]
        public void it_should_have_the_correct_controller_name()
        {
            Sut.ControllerName.ShouldBeEqualTo(_controllerName);
        }

        [Observation]
        public void should_have_a_controller_type()
        {
            Sut.ControllerType.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_a_ruby_controller_as_type()
        {
            Sut.ControllerType.ShouldBeEqualTo(typeof(RubyController));
        }

        [Observation]
        public void should_have_a_ruby_controller_class()
        {
            Sut.RubyControllerClass.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_a_ruby_engine()
        {
            Sut.RubyEngine.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_the_correct_ruby_controller_class()
        {
            Sut.RubyControllerClass.ShouldBeEqualTo(_rubyClass);
        }

        [Observation]
        public void should_have_an_empty_actions_list()
        {
            Sut.GetCanonicalActions().ShouldBeEmpty();
        }

    }
}