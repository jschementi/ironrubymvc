// REVIEW: This all uses Response.Write, should pass in the text writer instead?

#region Usings

using System;
using IronRubyMvcLibrary.Extensions;

#endregion

namespace IronRubyMvcLibrary.ViewEngine
{
    internal class RubyScriptBlock
    {
        [ThreadStatic] private static bool ignoreNextNewLine;

        private RubyScriptBlock(string block)
        {
            bool ignoreNewLine = ignoreNextNewLine;

            if (String.IsNullOrEmpty(block))
            {
                Contents = string.Empty;
                return;
            }

            int endOffset = 4;
            if (block.EndsWith("-%>", StringComparison.OrdinalIgnoreCase))
            {
                endOffset = 5;
                ignoreNextNewLine = true;
            }
            else
            {
                ignoreNextNewLine = false;
            }

            if (block.StartsWith("<%=", StringComparison.OrdinalIgnoreCase))
            {
                int outputLength = block.Length - endOffset - 1;
                if (outputLength < 1)
                    throw new InvalidOperationException("Started a '<%=' block without ending it.");

                string output = block.Substring(3, outputLength).Trim();
                Contents = "response.Write({0})".FormattedWith(output).Trim();
                return;
            }

            if (block.StartsWith("<%", StringComparison.OrdinalIgnoreCase))
            {
                Contents = block.Substring(2, block.Length - endOffset).Trim();
                return;
            }

            if (ignoreNewLine)
                block = block.Trim();

            block = block.Replace(@"\", @"\\");
            block = block.Replace(Environment.NewLine, "\\r\\n");
            block = block.Replace(@"""", @"\""");

            if (block.Length > 0)
                Contents = "response.Write(\"{0}\")".FormattedWith(block);
        }

        public string Contents { get; private set; }

        public static RubyScriptBlock Parse(string block)
        {
            return new RubyScriptBlock(block);
        }
    }
}