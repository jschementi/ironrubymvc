#region Usings

using IronRuby.Builtins;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class HashToAuthorizationFilterConverter : HashConverter<RubyAuthorizationFilter>
    {
        private static readonly SymbolId authorizeKey = SymbolTable.StringToId("authorize");

        public HashToAuthorizationFilterConverter()
        {
        }

        public HashToAuthorizationFilterConverter(Hash filterDescription) : base(filterDescription)
        {
        }

        protected override RubyAuthorizationFilter Build()
        {
            var authorize = FindProc(authorizeKey);
            return authorize.IsNull() ? null : new RubyAuthorizationFilter {Authorize = authorize};
        }

        protected override bool IsFilter()
        {
            return authorizeKey == (SymbolId) FilterDescription[whenKey];
        }
    }
}