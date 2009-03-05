#region Usings

using System;
using System.Web.Mvc.IronRuby.Core;
using IronRuby;
using IronRuby.Runtime;
using System.Web.Mvc.IronRuby.Core;
using Microsoft.Scripting.Hosting;
using Xunit;

#endregion

namespace System.Web.Mvc.IronRuby.Tests.Core
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

    [Concern(typeof (DefaultScriptRunner))]
    public class when_no_file_is_provided : with_ironruby_initialized<DefaultScriptRunner>
    {
        private Action _action;

        protected override DefaultScriptRunner CreateSut()
        {
            return new DefaultScriptRunner(_engine, string.Empty,
                                           new AssemblyResourceReader(typeof (RubyExperiments).Assembly));
        }

        protected override void Because()
        {
            _action = () => Sut.ExecuteFile(string.Empty);
        }

        [Observation]
        public void then_it_should_return_a_string()
        {
            _action.ShouldThrowAn<NullReferenceException>();
        }
    }

    [Concern(typeof (DefaultScriptRunner))]
    public class when_no_reader_is_provided : with_ironruby_initialized<DefaultScriptRunner>
    {
        private Action _action;

        protected override DefaultScriptRunner CreateSut()
        {
            return new DefaultScriptRunner(_engine, string.Empty, null);
        }

        protected override void Because()
        {
            _action = () => Sut.ExecuteFile("IronRubyMvcLibrary.Tests.Core.EmbeddedTestScript.rb");
        }

        [Observation]
        public void then_it_should_return_a_string()
        {
            _action.ShouldThrowAn<NullReferenceException>();
        }
    }


    [Concern(typeof (DefaultScriptRunner))]
    public class when_an_existing_file_is_provided : with_ironruby_initialized<DefaultScriptRunner>
    {
        private string result;

        protected override DefaultScriptRunner CreateSut()
        {
            return new DefaultScriptRunner(_engine, string.Empty,
                                           new AssemblyResourceReader(typeof (RubyExperiments).Assembly));
        }

        protected override void Because()
        {
            var obj = Sut.ExecuteFile("IronRubyMvcLibrary.Tests.Core.EmbeddedTestScript.rb");
            result = obj.ToString();
        }

        [Observation]
        public void then_it_should_return_a_string()
        {
            Assert.Equal("It works", result);
        }
    }

    [Concern(typeof (DefaultScriptRunner))]
    public class when_a_piece_of_script_is_provided : with_ironruby_initialized<DefaultScriptRunner>
    {
        private string result;

        protected override DefaultScriptRunner CreateSut()
        {
            return new DefaultScriptRunner(_engine, string.Empty,
                                           new AssemblyResourceReader(typeof (RubyExperiments).Assembly));
        }

        protected override void Because()
        {
            var obj = Sut.ExecuteScript("\"It works from script\".to_clr_string");
            result = obj.ToString();
        }

        [Observation]
        public void then_it_should_return_a_string()
        {
            Assert.Equal("It works from script", result);
        }
    }
}