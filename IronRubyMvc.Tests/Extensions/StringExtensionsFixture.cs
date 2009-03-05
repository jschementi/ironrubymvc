#region Usings

using System;
using System.Web.Mvc.IronRuby.Extensions;
using Xunit;

#endregion

namespace IronRubyMvc.Tests.Extensions
{
    public class StringExtensionsFixture
    {
        [Fact]
        public void ShouldReturnTrue_ForNullValue_WhenAskedForNullOrBlank()
        {
            Assert.True(((string) null).IsNullOrBlank());
        }

        [Fact]
        public void ShouldReturnTrue_ForEmptyValue_WhenAskedNullOrBlank()
        {
            var value = string.Empty;
            Assert.True(value.IsNullOrBlank());
        }

        [Fact]
        public void ShouldReturnTrue_ForOnlySpaces_WhenAskedNullOrBlank()
        {
            var value = "     ";
            Assert.True(value.IsNullOrBlank());
        }

        [Fact]
        public void ShouldReturnFalse_ForStringValue_WhenAskedNullOrBlank()
        {
            var value = "a string";
            Assert.False(value.IsNullOrBlank());
        }

        [Fact]
        public void ShouldReturnFalse_ForNullValue_WhenAskedForNotNullOrBlank()
        {
            Assert.False(((string) null).IsNotNullOrBlank());
        }

        [Fact]
        public void ShouldReturnFalse_ForEmptyValue_WhenAskedNotNullOrBlank()
        {
            var value = string.Empty;
            Assert.False(value.IsNotNullOrBlank());
        }

        [Fact]
        public void ShouldReturnFalse_ForOnlySpaces_WhenAskedNotNullOrBlank()
        {
            var value = "     ";
            Assert.False(value.IsNotNullOrBlank());
        }

        [Fact]
        public void ShouldReturnTrue_ForStringValue_WhenAskedNotNullOrBlank()
        {
            var value = "a string";
            Assert.True(value.IsNotNullOrBlank());
        }

        [Fact]
        public void ShouldFormatAStringProperly()
        {
            var expected = "This is the 1 and only Format test at " + DateTime.Now.ToShortDateString();

            var actual = "This is the {0} and only {1} test at {2}".FormattedWith(1, "Format",
                                                                                  DateTime.Now.ToShortDateString());

            Assert.Equal(expected, actual);
        }
    }
}