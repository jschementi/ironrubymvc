namespace IronRubyMvc.Tests {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RubyTemplateTests {
        [TestMethod]
        public void NullThrowsArgumentException() {
            UnitTestHelpers.AssertThrows<ArgumentNullException>(
                () => new RubyTemplate(null));
        }

        [TestMethod]
        public void CanParseEmptyTemplate() {
            RubyTemplate template = new RubyTemplate(string.Empty);
            Assert.AreEqual(string.Empty, template.ToScript());
        }

        [TestMethod]
        public void CanAddRequires() {
            RubyTemplate template = new RubyTemplate(string.Empty);
            template.AddRequire("mscorlib.dll");
            Assert.AreEqual(@"require 'mscorlib.dll'", template.ToScript());
        }

        [TestMethod]
        public void CanParseScriptBlock() {
            RubyTemplate template = new RubyTemplate("<% puts 'hello world' %>");

            Assert.AreEqual("puts 'hello world'", template.ToScript());
        }

        [TestMethod]
        public void ParsingAScriptBlockThatStartsButDoesNotEndThrowsInvalidOperationException() {
            RubyTemplate template = new RubyTemplate("<%=");
            UnitTestHelpers.AssertThrows<InvalidOperationException>(() => template.ToScript(), "Started a '<%=' block without ending it.");
        }

        [TestMethod]
        public void CanParseScriptWriteBlock() {
            RubyTemplate template = new RubyTemplate("<%= \"Hello World\" %>");
            string result = template.ToScript();
            Assert.AreEqual("response.Write(\"Hello World\")", result);
        }

        [TestMethod]
        public void CanParseMultiLineBlock() {
            RubyTemplate template = new RubyTemplate("<% puts \"Hello World\"\r\nputs 'IronRuby is fun!' %>");
            string result = template.ToScript();
            Assert.AreEqual("puts \"Hello World\"\r\nputs 'IronRuby is fun!'", result);
        }

        [TestMethod]
        public void LeavesHtmlAlone() {
            RubyTemplate template = new RubyTemplate("<html>\r\n<head><title></title></head>\r\n<body>\r\nHello World</body>\r\n</html>");
            string result = template.ToScript();
            Assert.AreEqual(ExpectedWrite("\"<html>\\r\\n<head><title></title></head>\\r\\n<body>\\r\\nHello World</body>\\r\\n</html>\""), result);
        }

        [TestMethod]
        public void CanParseHtmlAndScriptBlocks() {
            RubyTemplate template = new RubyTemplate("<html><% puts 'test' %></html>");
            string result = template.ToScript();

            string expected = ExpectedWrite("\"<html>\"") + Environment.NewLine
                + "puts 'test'" + Environment.NewLine
                + ExpectedWrite("\"</html>\"");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TemplateWithBackslashDoesNotCauseException() {
            RubyTemplate template = new RubyTemplate(@"<html>\</html>");
            string result = template.ToScript();

            string expected = ExpectedWrite(@"""<html>\\</html>""");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CanParseHtmlEndingInScriptBlock() {
            RubyTemplate template = new RubyTemplate("<html><% puts 'test' %>");
            string result = template.ToScript();

            string expected = ExpectedWrite("\"<html>\"") + Environment.NewLine
                + "puts 'test'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CanParseHtmlContainingDoubleQuotes() {
            RubyTemplate template = new RubyTemplate("<html><span title=\"blah\" /></html>");
            string result = template.ToScript();

            string expected = @"response.Write(""<html><span title=\""blah\"" /></html>"")";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CanParseHtmlBeginningInScriptBlock() {
            RubyTemplate template = new RubyTemplate("<% puts 'test' %></html>");
            string result = template.ToScript();

            string expected = "puts 'test'" + Environment.NewLine
                + ExpectedWrite("\"</html>\"");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NewLinesInTemplateAreReflectedInScript() {
            string original = "<% [1..10].each do |i| %>" + Environment.NewLine
                            + "	<%= i %>" + Environment.NewLine
                            + "<% end %>";

            RubyTemplate template = new RubyTemplate(original);
            string result = template.ToScript();

            string expected = "[1..10].each do |i|" + Environment.NewLine
                            + ExpectedWrite(@"""\r\n	""") + Environment.NewLine
                            + ExpectedWrite("i") + Environment.NewLine
                            + ExpectedWrite(@"""\r\n""") + Environment.NewLine
                            + "end";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CanRemoveNewlineAtEnd() {
            string original = "<% [1..10].each do |i| %>" + Environment.NewLine
                            + "	<%= i -%>" + Environment.NewLine
                            + "<% end %>";

            RubyTemplate template = new RubyTemplate(original);
            string result = template.ToScript();

            string expected = "[1..10].each do |i|" + Environment.NewLine
                            + ExpectedWrite(@"""\r\n	""") + Environment.NewLine
                            + ExpectedWrite("i") + Environment.NewLine
                            + "end";

            Assert.AreEqual(expected, result);
        }

        string ExpectedWrite(string s) {
            return string.Format("response.Write({0})", s);
        }
    }
}
