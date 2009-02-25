#region Usings

using IronRuby.Builtins;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Controllers
{
    public class HashToAuthorizationFilterConverter : HashConverter<RubyAuthorizationFilter>
    {
        private static readonly SymbolId authorizeKey = SymbolTable.StringToId("authorize");

        public HashToAuthorizationFilterConverter(Hash filterDescription) : base(filterDescription)
        {
        }

        protected override RubyAuthorizationFilter Build()
        {
            return new RubyAuthorizationFilter {Authorize = FindProc(authorizeKey)};
        }

        protected override bool IsFilter()
        {
            return authorizeKey == (SymbolId) _filterDescription[whenKey];
        }
    }
}