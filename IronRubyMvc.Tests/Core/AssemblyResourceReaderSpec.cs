using System;
using IronRubyMvc.Core;
using Xunit;

namespace IronRubyMvc.Tests.Core
{
    [Concern(typeof (AssemblyResourceReader))]
    public class when_reading_a_resource_and_an_existing_path_is_given : InstanceContextSpecification<AssemblyResourceReader>
    {
        private string _resourcePath;
        private string _result;

        protected override void EstablishContext()
        {
            _resourcePath = "IronRubyMvc.Tests.Core.EmbeddedTestResource.txt";
        }

        protected override AssemblyResourceReader CreateSut()
        {
            return new AssemblyResourceReader(typeof(when_reading_a_resource_and_an_existing_path_is_given).Assembly);
        }

        protected override void Because()
        {
            _result = Sut.Read(_resourcePath);
        }

        [Observation]
        public void it_should_read_the_contents_correctly()
        {
            _result.ShouldBeEqualTo("Content for TestResource.");
        }
    }

    [Concern(typeof (AssemblyResourceReader))]
    public class when_reading_a_resource_and_a_non_existing_path_is_given : InstanceContextSpecification<AssemblyResourceReader>
    {
        private string _resourcePath;
        private Action _action;
        private string _result;

        protected override void EstablishContext()
        {
            _resourcePath = "IronRubyMvc.Tests.Core.EmbeddedTestResource9876.txt";
        }

        protected override AssemblyResourceReader CreateSut()
        {
            return new AssemblyResourceReader(typeof(when_reading_a_resource_and_an_existing_path_is_given).Assembly);
        }

        protected override void Because()
        {
            _action = The.Action(() => _result = Sut.Read(_resourcePath));
            _action();
        }

        [Observation]
        public void should_not_throw_any_exceptions()
        {
            _action.ShouldNotThrowAnyExceptions();
        }

        [Observation]
        public void should_return_an_empty_string()
        {
            _result.ShouldBeEmpty();
        }


    }
}