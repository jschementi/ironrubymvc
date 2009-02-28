#region Usings

using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    public class HashToAuthorizationFilterConverter : HashConverter<RailsStyleAuthorizationFilter>
    {
        private static readonly SymbolId authorizeKey = SymbolTable.StringToId("authorize");

        public HashToAuthorizationFilterConverter()
        {
        }

        public HashToAuthorizationFilterConverter(Hash filterDescription) : base(filterDescription)
        {
        }

        protected override RailsStyleAuthorizationFilter Build()
        {
            var authorize = FindProc(authorizeKey);
            return authorize.IsNull() ? null : new RailsStyleAuthorizationFilter {Authorize = authorize};
        }

        protected override bool IsFilter()
        {
            return authorizeKey == (SymbolId) FilterDescription[whenKey];
        }
    }
}