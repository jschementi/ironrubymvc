using System;
using System.IO;
using System.Web.Hosting;
using IronRubyMvc.Core;
using Xunit;

namespace IronRubyMvc.Tests.Core
{
    [Concern(typeof (FileReader))]
    public class when_reading_a_file_and_an_existing_path_is_given : InstanceContextSpecification<FileReader>
    {
        private string _filePath;
        private string _result;
        private IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            _filePath = Path.Combine(Environment.CurrentDirectory, "Core\\TestFile.txt");
            _pathProvider = Dependency<IPathProvider>();

            _pathProvider.WhenToldTo(prov => prov.FileExists(_filePath)).Return(true);
            _pathProvider.WhenToldTo(prov => prov.Open(_filePath)).Return(new FileStream(_filePath, FileMode.Open));
        }

        protected override FileReader CreateSut()
        {
            return new FileReader(_pathProvider);
        }

        protected override void Because()
        {
            _result = Sut.Read(_filePath);
        }

        [Observation]
        public void it_should_read_the_contents_correctly()
        {
            _result.ShouldBeEqualTo("Content for TestResource.");
        }
    }

    [Concern(typeof(FileReader))]
    public class when_reading_a_file_and_a_non_existing_path_is_given : InstanceContextSpecification<FileReader>
    {
        private string _filePath;
        private Action _action;
        private string _result;
        private IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            _filePath = Path.Combine(Environment.CurrentDirectory, "Core\\TestFile2.txt");
            _pathProvider = Dependency<IPathProvider>();

            _pathProvider.WhenToldTo(prov => prov.FileExists(_filePath)).Return(false);
            //_pathProvider.WhenToldTo(prov => prov.Open(_filePath)).Return(string.Empty);
        }

        protected override FileReader CreateSut()
        {
            return new FileReader(_pathProvider);
        }

        protected override void Because()
        {
            _action = The.Action(() => _result = Sut.Read(_filePath));
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