using System.Diagnostics.CodeAnalysis;
using Microsoft.Scripting;

namespace IronRubyMvcLibrary.Helpers
{
    internal static class SymbolConstants
    {
        // Common filter description keys
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "When")]
        public static readonly SymbolId When = SymbolTable.StringToId("when");
        public static readonly SymbolId Type = SymbolTable.StringToId("type");
        public static readonly SymbolId Options = SymbolTable.StringToId("options");
        public static readonly SymbolId Name = SymbolTable.StringToId("name");

        // Symbols for rails style filters
        public static readonly SymbolId Before = SymbolTable.StringToId("before");
        public static readonly SymbolId After = SymbolTable.StringToId("after");
        public static readonly SymbolId Around = SymbolTable.StringToId("around");
        public static readonly SymbolId Authorize = SymbolTable.StringToId("authorize");
        public static readonly SymbolId Error = SymbolTable.StringToId("error");
        public static readonly SymbolId BeforeResult = SymbolTable.StringToId("before_result");
        public static readonly SymbolId AfterResult = SymbolTable.StringToId("after_result");

        // Symbols for supporting statically compiled filters and their inheritors
        public static readonly SymbolId Class = SymbolTable.StringToId("class");

    }
}