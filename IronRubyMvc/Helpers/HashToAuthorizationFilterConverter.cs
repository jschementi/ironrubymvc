#region Usings

using System.Web.Mvc;
using IronRuby.Builtins;
using IronRubyMvcLibrary.Controllers;
using IronRubyMvcLibrary.Core;
using IronRubyMvcLibrary.Extensions;
using Microsoft.Scripting;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    public class HashToAuthorizationFilterConverter : HashConverter<IAuthorizationFilter>
    {
        public HashToAuthorizationFilterConverter()
        {
        }

        public HashToAuthorizationFilterConverter(IRubyEngine rubyEngine) : base(rubyEngine)
        {
        }

        public HashToAuthorizationFilterConverter(IRubyEngine rubyEngine, Hash filterDescription) : base(rubyEngine, filterDescription)
        {
        }

        protected override IAuthorizationFilter Build()
        {
            var authorize = FindProc(SymbolConstants.Authorize);
            if(authorize.IsNotNull()) return new RailsStyleAuthorizationFilter {Authorize = authorize};

            var filterClass = FilterDescription[SymbolConstants.Class] as RubyClass;
            return filterClass.IsNotNull() ? _rubyEngine.CreateInstance<IAuthorizationFilter>(filterClass, false) : null;
        }

        protected override bool IsFilter()
        {
            return SymbolConstants.Authorize == (SymbolId)FilterDescription[SymbolConstants.When];
        }
    }
}