#region Usings

using IronRubyMvcLibrary.Extensions;
using Xunit;

#endregion

namespace IronRubyMvcLibrary.Tests.Extensions
{
    public class ObjectExtensionsFixture
    {
        [Fact]
        public void ShouldReturnTrueForNullObjectWhenCheckingForNull()
        {
            Assert.True(ObjectExtensions.IsNull(null));
        }

        [Fact]
        public void ShouldReturnFalseForObjectWhenCheckingForNull()
        {
            Assert.False(new object().IsNull());
        }

        [Fact]
        public void ShouldReturnFalseForNullObjectWhenCheckingForNotNull()
        {
            Assert.False(ObjectExtensions.IsNotNull(null));
        }

        [Fact]
        public void ShouldReturnTrueForObjectWhenCheckingForNotNull()
        {
            Assert.True(new object().IsNotNull());
        }
    }
}