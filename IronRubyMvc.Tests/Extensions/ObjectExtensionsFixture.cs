#region Usings

using System.Web.Mvc.IronRuby.Extensions;
using Xunit;

#endregion

namespace System.Web.Mvc.IronRuby.Tests.Extensions
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