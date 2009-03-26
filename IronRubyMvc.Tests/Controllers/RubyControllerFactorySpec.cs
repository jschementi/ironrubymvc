using System;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Routing;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Core;
using Moq;
using Moq.Mvc;
using Xunit;

namespace System.Web.Mvc.IronRuby.Tests.Controllers
{
//    [Concern(typeof (RubyControllerFactory))]
//    public class when_a_ruby_controller_needs_to_be_resolved : InstanceContextSpecification<RubyControllerFactory>
//    {
//        private IRubyEngine _rubyEngine;
//        private IControllerFactory _controllerFactory;
//        private RequestContext _requestContext;
//        private const string _controllerName = "my_controller";
//        private IController _controller;
//
//        protected override void EstablishContext()
//        {
//            _rubyEngine = Dependency<IRubyEngine>();
//            _controllerFactory = Dependency<IControllerFactory>();
//            _requestContext = new RequestContext(new HttpContextMock().Object, new RouteData());
//
//            _controllerFactory
//                .WhenToldTo(factory => factory.CreateController(_requestContext, _controllerName))
//                .Throw(new InvalidOperationException());
//            
//            _rubyEngine.WhenToldTo(eng => eng.LoadController(_requestContext, _controllerName)).Return(Dependency<RubyController>());
//            
//        }
//
//        protected override RubyControllerFactory CreateSut()
//        {
//            return new RubyControllerFactory(_controllerFactory, _rubyEngine);
//        }
//
//        protected override void Because()
//        {
//            _controller = Sut.CreateController(_requestContext, _controllerName);
//        }
//
//        [Observation]
//        public void should_have_returned_a_result()
//        {
//            _controller.ShouldNotBeNull();
//        }
//
//        [Observation]
//        public void should_have_returned_a_controller()
//        {
//            _controller.ShouldBeAnInstanceOf<IController>();
//        }
//
//        [Observation]
//        public void it_should_have_called_the_ruby_engine()
//        {
//            _rubyEngine.WasToldTo(eng => eng.LoadController(_requestContext, _controllerName)).OnlyOnce();
//        }
//
//        [Observation]
//        public void should_have_called_the_inner_controller_factory()
//        {
//            _controllerFactory.WasToldTo(factory => factory.CreateController(_requestContext, _controllerName)).OnlyOnce();
//        }
//    }

    [Concern(typeof(RubyControllerFactory))]
    public class when_a_ruby_controller_needs_to_be_disposed: InstanceContextSpecification<RubyControllerFactory>
    {
        private IRubyEngine _rubyEngine;
        private IControllerFactory _controllerFactory;
        private IController _controller;
        private IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            _rubyEngine = Dependency<IRubyEngine>();
            _controllerFactory = Dependency<IControllerFactory>();
            _pathProvider = Dependency<IPathProvider>();

            _controller = Dependency<RubyController>();
        }

        protected override RubyControllerFactory CreateSut()
        {
            return new RubyControllerFactory(_pathProvider, _controllerFactory, _rubyEngine);
        }

        protected override void Because()
        {
            Sut.ReleaseController(_controller);
        }

        [Observation]
        public void should_have_called_dispose()
        {
            _controller.WasToldTo(c => ((IDisposable)c).Dispose()).OnlyOnce();
        }
        
    }
}