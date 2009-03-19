#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Tests.Core;
using System.Web.Mvc.IronRuby.Tests.Core;
using System.Web.Routing;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Tests.Core;
using IronRuby;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;
using Moq.Mvc;
using Rhino.Mocks;
using Xunit;

#endregion

namespace System.Web.Mvc.IronRuby.Tests.Controllers
{
    public abstract class with_ironruby_initialized<SystemUnderTest> : InstanceContextSpecification<SystemUnderTest>
        where SystemUnderTest : class
    {
        protected static ScriptRuntime _scriptRuntime;
        protected RubyContext _context;
        protected ScriptEngine _engine;

        protected override void EstablishContext()
        {
            if (_scriptRuntime == null)
            {
                var rubySetup = Ruby.CreateRubySetup();
                rubySetup.Options["InterpretedMode"] = true;

                var runtimeSetup = new ScriptRuntimeSetup();
                runtimeSetup.LanguageSetups.Add(rubySetup);
                runtimeSetup.DebugMode = true;

                _scriptRuntime = Ruby.CreateRuntime(runtimeSetup);
            }
            _engine = _scriptRuntime.GetRubyEngine();
            _context = Ruby.GetExecutionContext(_engine);
        }
    }

    public abstract class with_ironruby_and_an_engine_initialized<SystemUnderTest> :
        with_ironruby_initialized<SystemUnderTest> where SystemUnderTest : class
    {
        protected IPathProvider _pathProvider;
        protected IRubyEngine _rubyEngine;

        protected override void EstablishContext()
        {
            base.EstablishContext();


            _pathProvider = Dependency<IPathProvider>();
            _pathProvider.WhenToldTo(pp => pp.ApplicationPhysicalPath).Return(Environment.CurrentDirectory);

            _rubyEngine = new RubyEngine(_scriptRuntime, _pathProvider);
        }
    }

    [Concern(typeof (RubyActionDescriptor))]
    public class when_descriptor_executes : with_ironruby_and_an_engine_initialized<RubyActionDescriptor>
    {
        private ControllerContext _controllerContext;
        private RubyControllerDescriptor _controllerDescriptor;
        private string _result;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            var script = new StringBuilder();
            script.AppendLine("class SamuraisController < Controller");
            script.AppendLine("  def my_action");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");
            _rubyEngine.ExecuteScript(script.ToString());

            var rubyClass = _rubyEngine.GetRubyClass("SamuraisController");
            var httpContext = new HttpContextMock().Object;
            var requestContext = new RequestContext(httpContext, new RouteData());
            var controller = _rubyEngine.ConfigureController(_rubyEngine.GetRubyClass("SamuraisController"),
                                                             requestContext);
            _controllerContext = new ControllerContext(requestContext, controller);

            _controllerDescriptor = new RubyControllerDescriptor(rubyClass, _rubyEngine);
        }

        protected override RubyActionDescriptor CreateSut()
        {
            return new RubyActionDescriptor("my_action", _controllerDescriptor, _rubyEngine);
        }

        protected override void Because()
        {
            _result = Sut.Execute(_controllerContext, new Dictionary<string, object>()).ToString();
        }

        [Observation]
        public void should_execute_the_action()
        {
            _result.ShouldBeEqualTo("Can't see ninjas");
        }
    }

    [Concern(typeof (RubyActionDescriptor))]
    public class when_descriptor_gets_initialized : with_ironruby_and_an_engine_initialized<RubyActionDescriptor>
    {
        private RubyControllerDescriptor _controllerDescriptor;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            var script = new StringBuilder();
            script.AppendLine("class SamuraisController < Controller");
            script.AppendLine("  def my_action");
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");
            _rubyEngine.ExecuteScript(script.ToString());

            _controllerDescriptor =
                MockRepository.GenerateStub<RubyControllerDescriptor>(_rubyEngine.GetRubyClass("SamuraisController"));
        }

        protected override RubyActionDescriptor CreateSut()
        {
            return new RubyActionDescriptor("my_action", _controllerDescriptor, _rubyEngine);
        }

        protected override void Because()
        {
        }

        [Observation]
        public void should_have_an_action_name()
        {
            Sut.ActionName.ShouldNotBeEmpty();
        }

        [Observation]
        public void should_have_the_correct_action_name()
        {
            Sut.ActionName.ShouldBeEqualTo("my_action");
        }

        [Observation]
        public void should_have_a_controller_descriptor()
        {
            Sut.ControllerDescriptor.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_a_ruby_controller_descriptor()
        {
            Sut.ControllerDescriptor.ShouldBeAnInstanceOf<RubyControllerDescriptor>();
        }

        [Observation]
        public void should_have_the_correct_controller_descriptor()
        {
            Assert.Same(Sut.ControllerDescriptor, _controllerDescriptor);
        }

        [Observation]
        public void should_return_an_empty_array_for_parameters()
        {
            Sut.GetParameters().ShouldBeEmpty();
        }
    }
}