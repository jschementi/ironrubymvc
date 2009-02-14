using System;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Xunit;

namespace IronRubyMvcLibrary.Tests.Core
{
    [Concern(typeof (RubyEngine))]
    public class when_asked_for_controller_class_name: StaticContextSpecification
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

    public abstract class with_an_engine_initialized : with_ironruby_initialized<RubyEngine>
    {
        protected IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _pathProvider = Dependency<IPathProvider>();    
        }

        protected override RubyEngine CreateSut()
        {
            return new RubyEngine(_scriptRuntime, _pathProvider);
        }

        protected void AddClass(string className)
        {
            string script =
                "class {0}; def my_method; $text_var = \"{1}\"; end; def another_method; $text_var = 'from other method'; end; end "
                    .FormattedWith("{0}Class".FormattedWith(className), className);
            _context.DefineGlobalVariable("text_var", "String value");
            
            Sut.ScriptRunner.ExecuteScript(script);
        }
    }

    [Concern(typeof (RubyEngine))]
    public class when_asked_for_an_existing_ruby_class : with_an_engine_initialized
    {
        private RubyClass _class;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _pathProvider.WhenToldTo(pp => pp.ApplicationPhysicalPath).Return(Environment.CurrentDirectory);
        }

        protected override void Because()
        {
            
            AddClass("ControllerTestBase");
            _class = Sut.GetRubyClass("ControllerTestBaseClass");

        }

        [Observation]
        public void it_should_be_able_to_get_the_ruby_class()
        {
            _class.Name.ShouldBeEqualTo("ControllerTestBaseClass");
        }
    }
}