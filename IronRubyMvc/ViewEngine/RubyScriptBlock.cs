// REVIEW: This all uses Response.Write, should pass in the text writer instead?

namespace IronRubyMvc {
    using System;

    internal class RubyScriptBlock {
        public static RubyScriptBlock Parse(string block) {
            return new RubyScriptBlock(block);
        }

        public string Contents { get; private set; }

        RubyScriptBlock(string block) {
            bool ignoreNewLine = ignoreNextNewLine;

            if (String.IsNullOrEmpty(block)) {
                Contents = string.Empty;
                return;
            }

            int endOffset = 4;
            if (block.EndsWith("-%>")) {
                endOffset = 5;
                ignoreNextNewLine = true;
            }
            else {
                ignoreNextNewLine = false;
            }

            if (block.StartsWith("<%=")) {
                int outputLength = block.Length - endOffset - 1;
                if (outputLength < 1)
                    throw new InvalidOperationException("Started a '<%=' block without ending it.");

                string output = block.Substring(3, outputLength).Trim();
                Contents = String.Format("response.Write({0})", output).Trim();
                return;
            }

            if (block.StartsWith("<%")) {
                Contents = block.Substring(2, block.Length - endOffset).Trim();
                return;
            }

            if (ignoreNewLine)
                block = block.Trim();

            block = block.Replace(@"\", @"\\");
            block = block.Replace(Environment.NewLine, "\\r\\n");
            block = block.Replace(@"""", @"\""");

            if (block.Length > 0)
                Contents = string.Format("response.Write(\"{0}\")", block);
        }

        [ThreadStatic]
        static bool ignoreNextNewLine = false;
    }
}