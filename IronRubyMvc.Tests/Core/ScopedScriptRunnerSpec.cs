#region Usings

using System;
using IronRubyMvcLibrary.Core;
using Xunit;

#endregion

namespace IronRubyMvcLibrary.Tests.Core
{
    [Concern(typeof (ScopedScriptRunner))]
    public class when_a_file_is_provided_to_scoped : with_ironruby_initialized<ScopedScriptRunner>
    {
        private string result;

        protected override ScopedScriptRunner CreateSut()
        {
            return new ScopedScriptRunner(_engine, _scriptRuntime.CreateScope(), string.Empty,
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

    [Concern(typeof (ScopedScriptRunner))]
    public class when_no_file_is_provided_to_scoped : with_ironruby_initialized<ScopedScriptRunner>
    {
        private Action _action;

        protected override ScopedScriptRunner CreateSut()
        {
            return new ScopedScriptRunner(_engine, _scriptRuntime.CreateScope(), string.Empty,
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

    [Concern(typeof (ScopedScriptRunner))]
    public class when_no_reader_is_provided_to_scoped : with_ironruby_initialized<ScopedScriptRunner>
    {
        private Action _action;

        protected override ScopedScriptRunner CreateSut()
        {
            return new ScopedScriptRunner(_engine, _engine.CreateScope(), string.Empty, null);
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


    [Concern(typeof (ScopedScriptRunner))]
    public class when_a_script_is_provided_to_scoped : with_ironruby_initialized<ScopedScriptRunner>
    {
        private string result;

        protected override ScopedScriptRunner CreateSut()
        {
            return new ScopedScriptRunner(_engine, _scriptRuntime.CreateScope(), string.Empty,
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