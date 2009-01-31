using IronRubyMvc.Core;
using Xunit;

namespace IronRubyMvc.Tests.Core
{
    [Concern(typeof (AssemblyResourceReader))]
    public class when_path_given : InstanceContextSpecification<AssemblyResourceReader>
    {
        protected override void EstablishContext()
        {

        }

        protected override AssemblyResourceReader CreateSut()
        {
            return new AssemblyResourceReader(typeof(when_path_given).Assembly);
        }

        protected override void Because()
        {

        }

        [Observation]
        public void first_observation()
        {

        }
    }
}