#region Usings

using System;
using IronRubyMvcLibrary.ViewEngine;
using Xunit;

#endregion

namespace IronRubyMvcLibrary.Tests
{
    public class RubyTemplateTests
    {
        [Fact]
        public void NullThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RubyTemplate(null));
        }

        [Fact]
        public void CanParseEmptyTemplate()
        {
            var template = new RubyTemplate(string.Empty);
            Assert.Equal(string.Empty, template.ToScript());
        }

        [Fact]
        public void CanAddRequires()
        {
            var template = new RubyTemplate(string.Empty);
            template.AddRequire("mscorlib.dll");
            Assert.Equal(@"require 'mscorlib.dll'", template.ToScript());
        }

        [Fact]
        public void CanParseScriptBlock()
        {
            var template = new RubyTemplate("<% puts 'hello world' %>");

            Assert.Equal("puts 'hello world'", template.ToScript());
        }

        [Fact]
        public void ParsingAScriptBlockThatStartsButDoesNotEndThrowsInvalidOperationException()
        {
            var template = new RubyTemplate("<%=");
            Assert.Throws<InvalidOperationException>("Started a '<%=' block without ending it.",
                                                     () => template.ToScript());
        }

        [Fact]
        public void CanParseScriptWriteBlock()
        {
            var template = new RubyTemplate("<%= \"Hello World\" %>");
            string result = template.ToScript();
            Assert.Equal("response.Write(\"Hello World\")", result);
        }

        [Fact]
        public void CanParseMultiLineBlock()
        {
            var template = new RubyTemplate("<% puts \"Hello World\"\r\nputs 'IronRuby is fun!' %>");
            string result = template.ToScript();
            Assert.Equal("puts \"Hello World\"\r\nputs 'IronRuby is fun!'", result);
        }

        [Fact]
        public void LeavesHtmlAlone()
        {
            var template =
                new RubyTemplate("<html>\r\n<head><title></title></head>\r\n<body>\r\nHello World</body>\r\n</html>");
            string result = template.ToScript();
            Assert.Equal(
                ExpectedWrite(
                    "\"<html>\\r\\n<head><title></title></head>\\r\\n<body>\\r\\nHello World</body>\\r\\n</html>\""),
                result);
        }

        [Fact]
        public void CanParseHtmlAndScriptBlocks()
        {
            var template = new RubyTemplate("<html><% puts 'test' %></html>");
            string result = template.ToScript();

            string expected = ExpectedWrite("\"<html>\"") + Environment.NewLine
                              + "puts 'test'" + Environment.NewLine
                              + ExpectedWrite("\"</html>\"");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TemplateWithBackslashDoesNotCauseException()
        {
            var template = new RubyTemplate(@"<html>\</html>");
            string result = template.ToScript();

            string expected = ExpectedWrite(@"""<html>\\</html>""");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanParseHtmlEndingInScriptBlock()
        {
            var template = new RubyTemplate("<html><% puts 'test' %>");
            string result = template.ToScript();

            string expected = ExpectedWrite("\"<html>\"") + Environment.NewLine
                              + "puts 'test'";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanParseHtmlContainingDoubleQuotes()
        {
            var template = new RubyTemplate("<html><span title=\"blah\" /></html>");
            string result = template.ToScript();

            string expected = @"response.Write(""<html><span title=\""blah\"" /></html>"")";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanParseHtmlBeginningInScriptBlock()
        {
            var template = new RubyTemplate("<% puts 'test' %></html>");
            string result = template.ToScript();

            string expected = "puts 'test'" + Environment.NewLine
                              + ExpectedWrite("\"</html>\"");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void NewLinesInTemplateAreReflectedInScript()
        {
            string original = "<% [1..10].each do |i| %>" + Environment.NewLine
                              + "	<%= i %>" + Environment.NewLine
                              + "<% end %>";

            var template = new RubyTemplate(original);
            string result = template.ToScript();

            string expected = "[1..10].each do |i|" + Environment.NewLine
                              + ExpectedWrite(@"""\r\n	""") + Environment.NewLine
                              + ExpectedWrite("i") + Environment.NewLine
                              + ExpectedWrite(@"""\r\n""") + Environment.NewLine
                              + "end";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanRemoveNewlineAtEnd()
        {
            string original = "<% [1..10].each do |i| %>" + Environment.NewLine
                              + "	<%= i -%>" + Environment.NewLine
                              + "<% end %>";

            var template = new RubyTemplate(original);
            string result = template.ToScript();

            string expected = "[1..10].each do |i|" + Environment.NewLine
                              + ExpectedWrite(@"""\r\n	""") + Environment.NewLine
                              + ExpectedWrite("i") + Environment.NewLine
                              + "end";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanConverToScript()
        {
            var template = new RubyTemplate("<% puts 'test' %></html>");
            string result = template.ToScript("puts_test");

            string expected = "def puts_test" + Environment.NewLine 
                              + "puts 'test'" + Environment.NewLine
                              + ExpectedWrite("\"</html>\"") + Environment.NewLine
                              + "end";

            Assert.Equal(expected, result);
        }

        private string ExpectedWrite(string s)
        {
            return "response.Write({0})".FormattedWith(s);
        }
    }
}