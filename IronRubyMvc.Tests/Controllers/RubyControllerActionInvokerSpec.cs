using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Routing;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Extensions;
using Moq.Mvc;
using Xunit;

namespace System.Web.Mvc.IronRuby.Tests.Controllers
{
    [Concern(typeof (RubyControllerActionInvoker))]
    public class when_the_action_invoker_is_initialized : with_ironruby_and_an_engine_initialized<RubyControllerActionInvoker>
    {
        private const string _controllerName = "SoldiersController";


        protected override RubyControllerActionInvoker CreateSut()
        {
            return new RubyControllerActionInvoker(_controllerName, _rubyEngine);
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
        public void should_have_the_correct_controller_name()
        {
            Sut.ControllerName.ShouldBeEqualTo(_controllerName);
        }

        [Observation]
        public void should_have_an_engine()
        {
            Sut.RubyEngine.ShouldNotBeNull();
        }
    }

    [Concern(typeof (RubyControllerActionInvoker))]
    public class when_asked_for_a_controller_descriptor : with_ironruby_and_an_engine_initialized<RubyControllerActionInvoker>
    {
        private const string _controllerName = "SoldiersController";
        private ControllerContext _controllerContext;
        private bool result;

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
             var rubyClass = _rubyEngine.GetRubyClass(_controllerName);

            var httpContext = new HttpContextMock().Object;
            var requestContext = new RequestContext(httpContext, new RouteData());
            var controller = _rubyEngine.CreateInstance<RubyController>(rubyClass);
            controller.InternalInitialize(new ControllerConfiguration { Context = requestContext, Engine = _rubyEngine, RubyClass = rubyClass });

            _controllerContext = new ControllerContext(requestContext, controller);
        }


        protected override RubyControllerActionInvoker CreateSut()
        {
            return new RubyControllerActionInvoker(_controllerName, _rubyEngine);
        }

        protected override void Because()
        {
            result = Sut.InvokeAction(_controllerContext, "my_action");
        }

        [Observation]
        public void should_execute_successfully()
        {
            result.ShouldBeTrue();
        }
    }
}