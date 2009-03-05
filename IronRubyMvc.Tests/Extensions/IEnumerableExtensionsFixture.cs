#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc.IronRuby.Extensions;
using Xunit;

#endregion

namespace IronRubyMvc.Tests.Extensions
{
    public class IEnumerableExtensionsFixture
    {
        [Fact]
        public void ShouldIterateOverAGenericCollection()
        {
            IEnumerable<int> collection = new[] {1, 2, 3, 4, 5, 6, 7, 8};

            var count = 0;
            var result = 0;
            collection.ForEach(item =>
                                   {
                                       count++;
                                       result += item;
                                   });

            Assert.Equal(8, count);
            Assert.Equal(36, result);
        }

        [Fact]
        public void ShouldIterateOverACollection()
        {
            IEnumerable collection = new[] {1, 2, 3, 4, 5, 6, 7, 8};

            var count = 0;
            var result = 0;
            collection.ForEach(item =>
                                   {
                                       count++;
                                       result += (int) item;
                                   });

            Assert.Equal(8, count);
            Assert.Equal(36, result);
        }
    }
}