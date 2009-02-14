using System.Collections.Generic;
using Xunit;

namespace IronRubyMvc.Tests.Extensions
{
    public class IEnumerableExtensionsFixture
    {
        [Fact]
        public void ShouldIterateOverACollection()
        {
            IEnumerable<int> collection = new[] {1, 2, 3, 4, 5, 6, 7, 8};

            int count=0;
            int result = 0;
            collection.ForEach(item =>
                                   {
                                       count++;
                                       result += item;
                                   });

            Assert.Equal(8, count);
            Assert.Equal(36, result);
        }
    }
}